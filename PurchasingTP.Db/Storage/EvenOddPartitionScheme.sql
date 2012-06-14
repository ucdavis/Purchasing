CREATE PARTITION SCHEME [EvenOddPartitionScheme]
    AS PARTITION [EvenOddPartitionFunction]
    TO ([PRIMARY], [Secondary]);

