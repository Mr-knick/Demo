def ComputeNewID(n, b):
    DigitList = [digit for digit in str(n)]
    DigitList.sort()
    DecDigitList = DigitList[::-1]
    y = int(''.join(DigitList), b)
    x = int(''.join(DecDigitList), b)
    z = x - y
    return ConvertB10toBb(z, b).zfill(len(str(n)))


def ConvertB10toBb(n, b):
    Digit = n // b
    Remainder = n % b
    if n == 0:
        return "0"
    elif Digit == 0:
        return str(Remainder)
    else:
        return str(ConvertB10toBb(Digit, b)) + str(Remainder)


def solution(n, b):
    DictOfIDs = {}
    CurrentID = n
    while int(CurrentID) != 0:
        if CurrentID in DictOfIDs:
            return int(len(DictOfIDs) - DictOfIDs[CurrentID] + 1)
        else:
            DictOfIDs[CurrentID] = len(DictOfIDs) + 1
            CurrentID = ComputeNewID(CurrentID, b)
    return int(1)

# print(solution('1211', 10))
# print(solution('210022', 3))
