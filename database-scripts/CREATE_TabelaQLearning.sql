USE [OnboardingDB]
GO

/****** Object:  Table [dbo].[TabelaQLearning]    Script Date: 08/06/2025 14:46:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TabelaQLearning](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EstadoId] [int] NOT NULL,
	[AcaoId] [int] NOT NULL,
	[ValorQ] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TabelaQLearning]  WITH CHECK ADD  CONSTRAINT [FK_TabelaQLearning_Acao] FOREIGN KEY([AcaoId])
REFERENCES [dbo].[Acao] ([Id])
GO

ALTER TABLE [dbo].[TabelaQLearning] CHECK CONSTRAINT [FK_TabelaQLearning_Acao]
GO

ALTER TABLE [dbo].[TabelaQLearning]  WITH CHECK ADD  CONSTRAINT [FK_TabelaQLearning_Estado] FOREIGN KEY([EstadoId])
REFERENCES [dbo].[Estado] ([Id])
GO

ALTER TABLE [dbo].[TabelaQLearning] CHECK CONSTRAINT [FK_TabelaQLearning_Estado]
GO


