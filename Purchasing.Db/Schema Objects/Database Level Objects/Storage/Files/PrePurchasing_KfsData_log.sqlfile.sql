ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [PrePurchasing_KfsData_log], FILENAME = '$(DefaultLogPath)$(DatabaseName)_KfsData_log.ndf', SIZE = 2048 KB, FILEGROWTH = 1024 KB) TO FILEGROUP [Secondary];





