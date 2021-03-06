SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Client](
	[Id] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](50) NULL,
	[ActivationCode] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[GoogleAuthCode] [nvarchar](250) NULL,
	[GoogleAuthActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Session]    Script Date: 12/11/2018 9:50:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Session](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ClientId] [uniqueidentifier] NOT NULL,
	[SessionKey] [nvarchar](50) NOT NULL,
	[IP] [nvarchar](50) NOT NULL,
	[Confirmed] [bit] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[ExpiredDate] [datetime] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Token]    Script Date: 12/11/2018 9:50:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Token](
	[Id] [uniqueidentifier] NOT NULL,
	[ClientId] [uniqueidentifier] NULL,
	[Value] [nvarchar](50) NULL,
	[AuthenticationMethod] [nvarchar](50) NULL,
	[IP] [nvarchar](50) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_Token] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Client] ADD  CONSTRAINT [DF_Client_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Token] ADD  CONSTRAINT [DF_Token_Id]  DEFAULT (newid()) FOR [Id]
GO
