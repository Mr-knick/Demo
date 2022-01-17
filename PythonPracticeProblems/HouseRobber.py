def rob(nums):
    """
    :type nums: List[int]
    :rtype: int
    """
    CostPerLocation = [0] * len(nums)
    LastBiggest = 0
    if len(nums) <= 2:
        return max(nums)
    CostPerLocation[0] = nums[0]
    CostPerLocation[1] = nums[1]

    for i in range(2, len(nums)):
        CostPerLocation[i] = max(CostPerLocation[i - 1], CostPerLocation[i - 2] + nums[i])

    return CostPerLocation[-1]


# print(rob([1,2,3,1]))
# print(rob([0]))
print(rob([2,1,1,2]))