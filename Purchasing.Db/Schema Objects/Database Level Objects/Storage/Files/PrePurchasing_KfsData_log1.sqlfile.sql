ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [PrePurchasing_KfsData_log1], FILENAME = '$(DefaultDataPath)PrePurchasing_KfsData_log1.ldf', SIZE = 5195968 KB, MAXSIZE = 2097152 MB, FILEGROWTH = 10 %);

