using System;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Linq;

using NAudio.Wave;


namespace SenseSignal
{
    class Program
    {
        // MICROPHONE ANALYSIS SETTINGS
        private static int RATE = 44100; // sample rate of the sound card (verified for my computer)
        private static int BUFFERSIZE = (int)Math.Pow(2, 11); // must be a multiple of 2

        // prepare class objects
        private static BufferedWaveProvider bwp;

        //stops dupicate threads
        private static Boolean ThreadRunning = false;

        //state checks
        private static Boolean State2 = false;
        private static Boolean State3 = false;
        private static Boolean ThreadKill = false;

        //MagicPacketMACAddress
        public static String MACAddress = null;

        private static void AudioDataAvailable(object sender, WaveInEventArgs e)
        {
            bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        private static void StartListeningToMicrophone(int audioDeviceNumber = 0)
        {
            WaveInEvent wi;
            wi = new WaveInEvent();

            wi.DeviceNumber = audioDeviceNumber;
            wi.WaveFormat = new NAudio.Wave.WaveFormat(RATE, 1);
            wi.BufferMilliseconds = (int)((double)BUFFERSIZE / (double)RATE * 1000.0);
            wi.DataAvailable += new EventHandler<WaveInEventArgs>(AudioDataAvailable);
            bwp = new BufferedWaveProvider(wi.WaveFormat);
            bwp.BufferLength = BUFFERSIZE * 2;
            bwp.DiscardOnBufferOverflow = true;
            try
            {
                wi.StartRecording();
            }
            catch
            {
                string msg = "Could not record from audio device!\n\n";
                msg += "Is your microphone plugged in?\n";
                msg += "Is it set as your default recording device?";
                Console.WriteLine(msg, "ERROR");
            }
        }

        private void Calculate()
        {
            // check the incoming microphone audio
            int frameSize = BUFFERSIZE;
            var audioBytes = new byte[frameSize];
            bwp.Read(audioBytes, 0, frameSize);

            // return if there's nothing new to plot
            if (audioBytes.Length == 0)
                return;
            if (audioBytes[frameSize - 2] == 0)
                return;

            // incoming data is 16-bit (2 bytes per audio point)
            int BYTES_PER_POINT = 2;

            // create a (32-bit) int array ready to fill with the 16-bit data
            int graphPointCount = audioBytes.Length / BYTES_PER_POINT;

            // create double arrays to hold the data we will graph
            // pcm will be time domain
            double[] pcm = new double[graphPointCount];
            double[] fft = new double[graphPointCount];
            double[] fftReal = new double[graphPointCount / 2];

            // populate Xs and Ys with double data
            for (int i = 0; i < graphPointCount; i++)
            {
                // read the int16 from the two bytes
                Int16 val = BitConverter.ToInt16(audioBytes, i * 2);

                // store the value in Ys as a percent (+/- 100% = 200%)
                pcm[i] = (double)(val) / Math.Pow(2, 16) * 200.0;
            }

            // calculate the full FFT
            fft = FFT(pcm);

            // count x axis
            double fftMaxFreq = RATE / 2;
            double fftPointSpacingHz = fftMaxFreq / (graphPointCount/2); //21.533203125 is the hz spacing

            // just keep the real half (the other half imaginary)
            Array.Copy(fft, fftReal, fftReal.Length);

            // filter noise for too low power
            for (int i = 0; i < fftReal.Length; i++)
            {
                if (fftReal[i] < .9)
                {
                    fftReal[i] = 0;
                }
            }

            for (int i = 0; i < fftReal.Length; i++)
            {
                //alternative is to set a lock object that has events fire when these conditions are met. 
                //state machine can also be hidden within an object instad of threads. used threads because it makes timers easier. 
                double CurrentFrequency = (double)i * fftPointSpacingHz;
                //low a and high c
                if (fftReal[i] > 0)
                {
                    if (CurrentFrequency == 301.46484375) //low a
                    {
                        if (!ThreadRunning)
                        {
                            //state1 begins by threadstart
                            Thread ThreadControllerThread = new Thread(ThreadController);
                            ThreadControllerThread.Start();
                        }
                        else if(State2)
                        {
                            State3 = true;
                        }
                    }
                    else if(CurrentFrequency == 4306.640625) //high c
                    {
                        State2 = true;
                    }
                }
            }
        }

        private double[] FFT(double[] data)
        {
            double[] fft = new double[data.Length];
            System.Numerics.Complex[] fftComplex = new System.Numerics.Complex[data.Length];
            for (int i = 0; i < data.Length; i++)
                fftComplex[i] = new System.Numerics.Complex(data[i], 0.0).Magnitude;
            Accord.Math.FourierTransform.FFT(fftComplex, Accord.Math.FourierTransform.Direction.Forward);
            //efficency by removing second for loop but less readable
            for (int i = 0; i < data.Length; i++)
                fft[i] = fftComplex[i].Magnitude;
            return fft;
        }

        private static void ThreadController()
        {
            ThreadRunning = true;
            Thread StateThread = new Thread(State);
            StateThread.Start();
            Thread.Sleep(2000);

            ThreadRunning = false;
            State2 = false;
            State3 = false;

            ThreadKill = true;
            return;
        }

        private static void State()
        {
            while(true)
            {
                if (State2 & State3)
                {
                    SendWakeOnLan(PhysicalAddress.Parse(MACAddress));
                    State2 = State3 = false;
                }
                if (ThreadKill)
                {
                    ThreadKill = false;
                    return;
                }
            }
        }

        public static void SendWakeOnLan(PhysicalAddress target)
        {
            var header = Enumerable.Repeat((byte)0xff, 6);
            var data = Enumerable.Repeat(target.GetAddressBytes(), 16).SelectMany(mac => mac);

            var magicPacket = header.Concat(data).ToArray();

            using var client = new UdpClient();

            client.Send(magicPacket, magicPacket.Length, new IPEndPoint(IPAddress.Broadcast, 9));

            Console.WriteLine("Turn on Command Sent: " + System.DateTime.Now);
        }

        public void Driver()
        {
            while (true)
            {
                Calculate();
            }
        }

        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                MACAddress = "00-00-00-00-00-00";
            }
            else
            {
                try
                {
                    if (args.Length > 0)
                    {
                        MACAddress = args[0];
                        Regex rgx = new Regex(@"^(?:[0-9a-fA-F]{2}:){5}[0-9a-fA-F]{2}|(?:[0-9a-fA-F]{2}-){5}[0-9a-fA-F]{2}|(?:[0-9a-fA-F]{2}){5}[0-9a-fA-F]{2}$");
                        if (!rgx.IsMatch(MACAddress))
                        {
                            Console.WriteLine("Invalid MAC address format.\n");
                            Console.ReadLine();
                            Environment.Exit(0);
                        }
                    }
                }
                catch (InvalidCastException e)
                {
                    Console.WriteLine("Invalid MAC address format.\n" + e);
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
            Console.WriteLine("Listening for unlock command. WOL set for: " + MACAddress);
            
            Program p = new Program();
            StartListeningToMicrophone();
            p.Driver();
        }
    }
}
