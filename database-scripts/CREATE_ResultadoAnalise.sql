USE [OnboardingDB]
GO

/****** Object:  Table [dbo].[ResultadoAnalise]    Script Date: 08/06/2025 14:42:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ResultadoAnalise](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Semana] [varchar](10) NOT NULL,
	[DataAnalise] [datetime] NOT NULL,
	[PeriodoInicio] [datetime] NOT NULL,
	[PeriodoFim] [datetime] NOT NULL,
	[MediaFitnessGenetico] [float] NOT NULL,
	[MelhorFitnessGenetico] [float] NOT NULL,
	[MelhorAlgoritmo] [varchar](50) NOT NULL,
	[Detalhes] [varchar](500) NULL,
	[FitnessDaMelhorEstrategiaQLearning] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ResultadoAnalise] ADD  DEFAULT ((0)) FOR [FitnessDaMelhorEstrategiaQLearning]
GO


