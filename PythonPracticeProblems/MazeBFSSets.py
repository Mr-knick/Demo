class Matrix():
    def __init__(self):
        self.MatrixDict = {}
        self.MaxLen = 0
        self.ShortestLen = 0
        self.Xlen = 0
        self.Ylen = 0

    def SetUp(self, Matrix):
        self.Xlen = len(Matrix[0])
        self.Ylen = len(Matrix)
        self.MaxLen = self.Xlen * self.Ylen
        self.ShortestLen = self.Xlen + self.Ylen - 1
        self.CurrentBest = self.MaxLen
        for i in range(len(Matrix[0])):
            for j in range(len(Matrix)):
                self.MatrixDict[(i,j)] = Cell(i, j, self.ValidMoves(i,j), Matrix[j][i])
        self.SetStartFinish()

    def SetStartFinish(self):
        self.MatrixDict[(0,0)].IsStart = True
        self.MatrixDict[(self.Xlen - 1, self.Ylen - 1)].IsFinish = True

    def ValidMoves(self, i, j):
        # Default Value and change if needed vs lots of else statements
        AvailableMoves = [('U', (i, j - 1)), ('D', (i, j + 1)), ('L', (i - 1, j)), ('R', (i + 1, j))]
        if AvailableMoves[0][1][1] < 0:
            AvailableMoves[0] = ('U', False)
        if AvailableMoves[1][1][1] == self.Ylen:
            AvailableMoves[1] = ('D', False)
        if AvailableMoves[2][1][0] < 0:
            AvailableMoves[2] = ('L', False)
        if AvailableMoves[3][1][0] == self.Xlen:
            AvailableMoves[3] = ('R', False)
        return {C for C in AvailableMoves if C[1]}

    def IsBest(self):
        return self.CurrentBest == self.ShortestLen

    def UpdateBest(self, CurrentRunLen):
        self.CurrentBest = min(self.CurrentBest, CurrentRunLen)


class Cell():
    def __init__(self, I, J, AvailableMoves, Cost):
        self.AvailableMoves = AvailableMoves
        self.Cost = Cost
        self.I = I
        self.J = J
        self.IsStart = False
        self.IsFinish = False

    def __str__(self):
        return str(tuple((self.I, self.J)))


def MinimalPathComputation(Mat):
    PathLenToCellDict = {}
    # Key is distance to cell
    # Value is all cells that are at that distance
    # A cell can have two distance values as it can be taking different paths
    # This methods doesn't preserve the route itself and only returns the value of shortest path
    PathLenToCellDict[1] = {Mat.MatrixDict[(0, 0)]}
    PathLen = 2
    # each interation expands the known node distances by one
    while PathLen < Mat.MaxLen and PathLen < Mat.CurrentBest:
        CurrentCellSet = set()
        NewCells = set()
        for CurrentCell in PathLenToCellDict[PathLen - 1]:
            for TestCellName in CurrentCell.AvailableMoves:
                TestCell = Mat.MatrixDict[TestCellName[1]]
                if not any(TestCell in S for S in PathLenToCellDict.values()) and TestCell.Cost == 0:
                    NewCells.add(TestCell)

        CurrentCellSet.update(NewCells)

        if any(C.IsFinish for C in CurrentCellSet):
            Mat.UpdateBest(PathLen)

        PathLenToCellDict[PathLen] = CurrentCellSet

        PathLen += 1
    # print(PathLen)
    return False


def solution(map):
    Mat = Matrix()
    Mat.SetUp(map)

    # Test unmodified first
    MinimalPathComputation(Mat)
    if Mat.IsBest():
        return Mat.ShortestLen

    # replace each wall one at a time
    for C in Mat.MatrixDict.values():
        if C.Cost == 1:
            C.Cost = 0
            MinimalPathComputation(Mat)
            if Mat.IsBest():
                return Mat.ShortestLen
            C.Cost = 1

    return Mat.CurrentBest



map =[[0, 0, 0, 0, 0, 0, 0, 0, 0],
      [0, 0, 0, 0, 0, 0, 1, 1, 0],
      [1, 1, 1, 1, 1, 0, 1, 1, 0],
      [1, 1, 1, 1, 1, 0, 1, 1, 1],
      [0, 0, 0, 0, 0, 0, 1, 1, 0],
      [1, 1, 1, 1, 1, 1, 1, 1, 0],
      [0, 1, 1, 1, 1, 1, 1, 0, 0],
      [0, 0, 0, 0, 0, 0, 0, 0, 0] ]

map = [ [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
      [0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0],
      [1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0],
      [1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1],
      [0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0],
      [1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0],
      [0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0],
      [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0],
        [1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0],
        [1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1],
        [0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0],
        [1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0],
        [0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        ]

# map =  [[0, 1, 0, 0],
#         [0, 0, 1, 0],
#         [0, 0, 1, 0],
#         [0, 1, 1, 0]]

# map =  [[0, 0, 0, 0],
#         [0, 0, 1, 0]]

# map =  [[0],
#         [1]]
#
# map =  [[0]]

import time
start_time = time.time()
print(solution(map))
print("--- %s seconds ---" % (time.time() - start_time))
