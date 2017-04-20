USE [DataExchange_Test]
GO

/****** Object:  Table [dbo].[PriceTable]    Script Date: 19.04.2017 16:36:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PriceTable](
	[MsgId] [bigint] NOT NULL,
	[ItemId] [varchar](50) NOT NULL,
	[ItemGroupId] [nvarchar](50) NOT NULL,
	[Price] [decimal](18, 4) NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PriceTable]  WITH CHECK ADD  CONSTRAINT [FK_PriceTable_MsgTable] FOREIGN KEY([MsgId])
REFERENCES [dbo].[MsgTable] ([MsgId])
GO

ALTER TABLE [dbo].[PriceTable] CHECK CONSTRAINT [FK_PriceTable_MsgTable]
GO

