USE [PrePurchasing]
GO

DROP PARTITION FUNCTION EvenOddPartitionFunction

CREATE PARTITION FUNCTION EvenOddPartitionFunction (int)
AS RANGE LEFT FOR VALUES (1)

CREATE PARTITION SCHEME EvenOddPartitionScheme
AS PARTITION EvenOddPartitionFunction
TO ( [PRIMARY], [Secondary] );
