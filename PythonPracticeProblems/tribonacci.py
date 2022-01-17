def tribonacci(n):
    """
    :type n: int
    :rtype: int
    """
    if n == 0:
        return 0
    elif n == 1:
        return 1
    elif n == 2:
        return 1

    AVC = [0] * (n + 1)
    AVC[1] = 1
    AVC[2] = 1


    for i in range(3, n+1):
        AVC[i] = AVC[i - 1] + AVC[i - 2] + AVC[i - 3]

    return AVC[-1]

print(tribonacci(3))