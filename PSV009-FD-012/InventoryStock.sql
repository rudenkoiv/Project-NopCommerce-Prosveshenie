USE [DataExchange_Test]
GO

/****** Object:  Table [dbo].[InventoryStock]    Script Date: 19.04.2017 16:49:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InventoryStock](
	[MsgId] [bigint] NOT NULL,
	[ItemId] [varchar](50) NOT NULL,
	[ItemGroupId] [varchar](50) NOT NULL,
	[AvailQty] [int] NULL,
	[ExpectedReceiptDate] [datetime] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[InventoryStock]  WITH CHECK ADD  CONSTRAINT [FK_InventoryStock_MsgTable] FOREIGN KEY([MsgId])
REFERENCES [dbo].[MsgTable] ([MsgId])
GO

ALTER TABLE [dbo].[InventoryStock] CHECK CONSTRAINT [FK_InventoryStock_MsgTable]
GO

