ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [PrePurchasing_log], FILENAME = 'E:\DB\PrePurchasing_log.ldf', SIZE = 10087872 KB, MAXSIZE = 2097152 MB, FILEGROWTH = 10 %);

