ALTER TABLE [dbo].[HistoryReceivedLineItems]
    ADD CONSTRAINT [DF_HistoryReceivedLineItems_CommentsUpdated] DEFAULT ((0)) FOR [CommentsUpdated];

