Public Module PrimeNumbers

    Public Function Primes(upTo As Integer) As List(Of Integer)
        Return Enumerable.Range(2, upTo + 1).AsParallel().Where(Function(n) IsPrime(n)).ToList()
    End Function

    Public Function IsPrime(n As Integer) As Boolean
        Return Not Enumerable.Range(2, n / 2).Any(Function(f) n Mod f = 0)
    End Function

    Public Function IsFactor(f As Integer, ofN As Integer)
        Return ofN Mod f = 0
    End Function

End Module
