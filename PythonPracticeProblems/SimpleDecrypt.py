def solution(x):
    # if type(x) == str():
    print(isinstance('',(x)))
    LenOfInputString = len(x)
    DecryptedList = [None] * LenOfInputString
    # 'while' loops are faster than 'for' loops
    i = 0
    while i < LenOfInputString:
        CharIntValue = (ord(x[i]))
        DecryptedList[i] = chr(25 - (CharIntValue - 97) + 97) if CharIntValue >= 97 and CharIntValue <= 122 else x[i]
        i += 1
    return ''.join(DecryptedList)


print(solution('This long string'))
