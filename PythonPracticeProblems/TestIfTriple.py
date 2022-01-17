def TestIfTriple(last, second, base):
    if last % second == 0:
        if second % base == 0:
            if last == 1:
                return True
            elif second == 1:
                return True
            elif base == 1:
                return True
            if last != second and second != base:
                return True
            else:
                return False
        else:
            return False
    else:
        return False


def SlowSolution(l):
    # No test conditions
    if len(l) < 3:
        return 0
    # Count = 0
    # test 2
    if l == [1, 1, 1]:
        return 1
    SetOfGoodValues = set()

    l.sort(reverse=True)
    Length = len(l)
    i = 0
    while i < Length:
        j = i + 1
        while j < Length:
            k = j + 1
            while k < Length:
                if TestIfTriple(l[i], l[j], l[k]):
                    # Count += 1
                    print('(%s, %s, %s)' % (l[i], l[j], l[k]))
                    SetOfGoodValues.add((l[i], l[j], l[k]))
                k += 1
            j += 1
        i += 1
        # print(i)
    return len(SetOfGoodValues)


def QuickSolution(l):
    Counter = 0
    Length = len(l)
    MultiplesByListPosition = [0] * Length

    # Break into 2 segments since triplets would make 3 if quad and so on
    numeratorIndex = 1
    while numeratorIndex < Length - 1:
        denominatorIndex = 0
        while denominatorIndex < numeratorIndex:
            if l[numeratorIndex] % l[denominatorIndex] == 0:
                MultiplesByListPosition[numeratorIndex] += 1
            denominatorIndex += 1
        numeratorIndex += 1

    numeratorIndex = 2
    while numeratorIndex < Length:
        denominatorIndex = 1
        while denominatorIndex < numeratorIndex:
            if l[numeratorIndex] % l[denominatorIndex] == 0:
                # if 0 won't add any
                # if 1 then this would add second therefor is triple and we increment
                # if we had a quad we would add another here and implied if here would be explicit there
                CurrentIndexValue = MultiplesByListPosition[denominatorIndex]
                Counter += CurrentIndexValue
            denominatorIndex += 1
        numeratorIndex += 1

    return Counter

