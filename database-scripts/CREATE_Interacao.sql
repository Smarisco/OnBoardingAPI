USE [OnboardingDB]
GO

/****** Object:  Table [dbo].[Interacao]    Script Date: 08/06/2025 14:42:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Interacao](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EstagiarioId] [int] NOT NULL,
	[EstadoAtualId] [int] NOT NULL,
	[AcaoTomadaId] [int] NOT NULL,
	[ProximoEstadoId] [int] NOT NULL,
	[RecompensaRecebida] [float] NOT NULL,
	[DataInteracao] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Interacao] ADD  DEFAULT (getdate()) FOR [DataInteracao]
GO

ALTER TABLE [dbo].[Interacao]  WITH CHECK ADD  CONSTRAINT [FK_Interacao_AcaoTomada] FOREIGN KEY([AcaoTomadaId])
REFERENCES [dbo].[Acao] ([Id])
GO

ALTER TABLE [dbo].[Interacao] CHECK CONSTRAINT [FK_Interacao_AcaoTomada]
GO

ALTER TABLE [dbo].[Interacao]  WITH CHECK ADD  CONSTRAINT [FK_Interacao_EstadoAtual] FOREIGN KEY([EstadoAtualId])
REFERENCES [dbo].[Estado] ([Id])
GO

ALTER TABLE [dbo].[Interacao] CHECK CONSTRAINT [FK_Interacao_EstadoAtual]
GO

ALTER TABLE [dbo].[Interacao]  WITH CHECK ADD  CONSTRAINT [FK_Interacao_Estagiario] FOREIGN KEY([EstagiarioId])
REFERENCES [dbo].[Estagiario] ([Id])
GO

ALTER TABLE [dbo].[Interacao] CHECK CONSTRAINT [FK_Interacao_Estagiario]
GO

ALTER TABLE [dbo].[Interacao]  WITH CHECK ADD  CONSTRAINT [FK_Interacao_ProximoEstado] FOREIGN KEY([ProximoEstadoId])
REFERENCES [dbo].[Estado] ([Id])
GO

ALTER TABLE [dbo].[Interacao] CHECK CONSTRAINT [FK_Interacao_ProximoEstado]
GO


