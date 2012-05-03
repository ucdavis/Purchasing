﻿ALTER TABLE [dbo].[vSubAccounts]
    ADD CONSTRAINT [PK_vSubAccounts] PRIMARY KEY CLUSTERED ([AccountNumber] ASC, [SubAccountNumber] ASC, [PartitionColumn] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


