ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [PrePurchasing_KfsData], FILENAME = 'E:\DB\PrePurchasing_KfsData.ndf', SIZE = 328704 KB, FILEGROWTH = 1024 KB) TO FILEGROUP [Secondary];

