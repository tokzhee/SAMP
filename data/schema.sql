
USE [SalaryAccountManagementPortal]
GO
/****** Object:  User [NT AUTHORITY\SYSTEM]    Script Date: 13-Jun-22 11:46:38 PM ******/
CREATE USER [NT AUTHORITY\SYSTEM] FOR LOGIN [NT AUTHORITY\SYSTEM] WITH DEFAULT_SCHEMA=[dbo]
GO
sys.sp_addrolemember @rolename = N'db_owner', @membername = N'NT AUTHORITY\SYSTEM'
GO
/****** Object:  Table [dbo].[app_activity]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[app_activity](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[audit_username] [varchar](50) NOT NULL,
	[audit_fullname] [varchar](100) NOT NULL,
	[audit_ipaddress] [varchar](50) NOT NULL,
	[audit_macaddress] [varchar](50) NULL,
	[operation] [varchar](100) NOT NULL,
	[comments] [varchar](max) NOT NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
 CONSTRAINT [PK_app_activity] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[email_log]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[email_log](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[subject] [varchar](max) NOT NULL,
	[message] [varchar](max) NOT NULL,
	[is_message_html_body] [bit] NOT NULL,
	[recipients] [varchar](max) NOT NULL,
	[copy] [varchar](max) NULL,
	[blind_copy] [varchar](max) NULL,
	[logged_on] [datetime] NOT NULL,
	[logged_by] [varchar](50) NOT NULL,
	[send_attempts] [bigint] NULL,
	[last_modified_on] [datetime] NULL,
	[last_modified_by] [varchar](50) NULL,
	[sent_flag] [bit] NOT NULL,
	[sent_on] [datetime] NULL,
	[sent_by] [varchar](50) NULL,
 CONSTRAINT [PK_email_log] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[employer]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[employer](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[employer_name] [varchar](500) NOT NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
	[approved_on] [datetime] NULL,
	[approved_by] [varchar](100) NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_employer] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[fintech]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fintech](
	[id] [bigint] NOT NULL,
	[corporate_name] [varchar](max) NOT NULL,
	[official_email_address] [varchar](150) NOT NULL,
	[head_office_address] [varchar](max) NULL,
	[relationship_manager_staff_id] [varchar](50) NOT NULL,
	[relationship_manager_person_id] [bigint] NOT NULL,
	[relationship_manager_sol_id] [varchar](50) NOT NULL,
	[relationship_manager_sol_name] [varchar](100) NOT NULL,
	[relationship_manager_sol_address] [varchar](max) NOT NULL,
	[account_number] [varchar](50) NOT NULL,
	[account_name] [varchar](max) NOT NULL,
	[finacle_term_id] [varchar](50) NOT NULL,
	[fee_scale] [varchar](50) NOT NULL,
	[scale_value] [decimal](18, 2) NOT NULL,
	[cap_amount] [decimal](18, 2) NOT NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
	[approved_on] [datetime] NULL,
	[approved_by] [varchar](100) NULL,
	[last_modified_on] [datetime] NULL,
	[last_modified_by] [varchar](100) NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_fintech] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[fintech_contact_persons]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fintech_contact_persons](
	[id] [bigint] NOT NULL,
	[fintech_id] [bigint] NOT NULL,
	[person_id] [bigint] NOT NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_fintech_contact_persons] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[fintech_fee_operation_log]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fintech_fee_operation_log](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[fintech_id] [bigint] NOT NULL,
	[operation_id] [int] NOT NULL,
	[operation_data] [varchar](max) NOT NULL,
	[initiated_on] [datetime] NOT NULL,
	[initiated_by] [varchar](100) NOT NULL,
	[operation_status_id] [int] NOT NULL,
	[operated_on] [datetime] NULL,
	[operated_by] [varchar](100) NULL,
 CONSTRAINT [PK_fintech_fee_operation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[fintech_onboarding_documents]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fintech_onboarding_documents](
	[id] [bigint] NOT NULL,
	[fintech_id] [bigint] NOT NULL,
	[document_id] [bigint] NOT NULL,
	[document_save_path] [varchar](max) NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
	[approved_on] [datetime] NULL,
	[approved_by] [varchar](100) NULL,
	[last_modified_on] [datetime] NULL,
	[last_modified_by] [varchar](100) NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_fintech_onboarding_documents] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[forgot_password_log]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[forgot_password_log](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[person_id] [bigint] NOT NULL,
	[username] [varchar](50) NOT NULL,
	[forgot_password_code] [varchar](150) NOT NULL,
	[forgot_password_generated_date] [datetime] NOT NULL,
 CONSTRAINT [PK_forgot_password_log] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[main_menu]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[main_menu](
	[id] [bigint] NOT NULL,
	[display_name] [varchar](50) NOT NULL,
	[menu_icon_id] [bigint] NOT NULL,
	[active_flag] [bit] NOT NULL,
	[arrangement_order] [int] NOT NULL,
 CONSTRAINT [PK_main_menu] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[maker_checker_log]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[maker_checker_log](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[maker_checker_category_id] [int] NOT NULL,
	[maker_checker_type_id] [int] NOT NULL,
	[action_name] [varchar](100) NOT NULL,
	[action_details] [varchar](max) NOT NULL,
	[action_data] [varchar](max) NOT NULL,
	[maker_person_id] [bigint] NOT NULL,
	[maker_username] [varchar](100) NOT NULL,
	[maker_fullname] [varchar](100) NOT NULL,
	[maker_sol_id] [varchar](50) NULL,
	[maker_sol_name] [varchar](100) NULL,
	[maker_sol_address] [varchar](500) NULL,
	[maker_checker_status] [int] NOT NULL,
	[date_made] [datetime] NOT NULL,
	[checker_person_id] [bigint] NULL,
	[checker_username] [varchar](100) NULL,
	[checker_fullname] [varchar](100) NULL,
	[checker_sol_id] [varchar](50) NULL,
	[checker_sol_name] [varchar](100) NULL,
	[checker_sol_address] [varchar](500) NULL,
	[checker_remarks] [varchar](max) NULL,
	[date_checked] [datetime] NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_maker_checker_log] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[menu_icon]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[menu_icon](
	[id] [bigint] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[active_flag] [bit] NOT NULL,
 CONSTRAINT [PK_menu_icon] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[onboarding_documents]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[onboarding_documents](
	[id] [bigint] NOT NULL,
	[document_name] [varchar](max) NOT NULL,
	[mandatory_flag] [bit] NOT NULL,
	[active_flag] [bit] NOT NULL,
 CONSTRAINT [PK_onboarding_documents] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[password_change_log]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[password_change_log](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[person_id] [bigint] NOT NULL,
	[username] [varchar](50) NOT NULL,
	[password] [varchar](50) NOT NULL,
	[logged_on] [datetime] NOT NULL,
 CONSTRAINT [PK_password_change_log] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[person]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[person](
	[id] [bigint] NOT NULL,
	[surname] [varchar](50) NOT NULL,
	[first_name] [varchar](50) NOT NULL,
	[middle_name] [varchar](50) NULL,
	[mobile_number] [varchar](50) NULL,
	[email_address] [varchar](150) NULL,
	[passport] [varchar](max) NULL,
	[signature] [varchar](500) NULL,
	[person_type_id] [int] NOT NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
	[last_modified_on] [datetime] NULL,
	[last_modified_by] [varchar](100) NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_person] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[person_type]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[person_type](
	[id] [int] NOT NULL,
	[person_type_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_person_type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[role_menu]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[role_menu](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[role_id] [bigint] NOT NULL,
	[menu_sub_id] [bigint] NOT NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
 CONSTRAINT [PK_role_menu] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[roles]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[roles](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[role_name] [varchar](50) NOT NULL,
	[active_flag] [bit] NOT NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
	[approved_on] [datetime] NULL,
	[approved_by] [varchar](100) NULL,
	[last_modified_on] [datetime] NULL,
	[last_modified_by] [varchar](100) NULL,
	[system_reserved_flag] [bit] NOT NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_roles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[salary_accounts]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[salary_accounts](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[account_number] [varchar](10) NOT NULL,
	[account_name] [varchar](500) NOT NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
	[approved_on] [datetime] NULL,
	[approved_by] [varchar](100) NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_salary_accounts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[salary_accounts_employers]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[salary_accounts_employers](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[salary_account_id] [bigint] NOT NULL,
	[employer_id] [bigint] NOT NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
	[approved_on] [datetime] NULL,
	[approved_by] [varchar](100) NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_salary_accounts_employers] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[salary_accounts_rac_profiling]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[salary_accounts_rac_profiling](
	[id] [bigint] NOT NULL,
	[salary_account_id] [bigint] NOT NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
	[approved_on] [datetime] NULL,
	[approved_by] [varchar](100) NULL,
	[rac_profiled_status_id] [int] NULL,
	[rac_profiled_on] [datetime] NULL,
	[rac_profiled_by] [varchar](100) NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_salary_accounts_rac_profiling] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[salary_payment_history]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[salary_payment_history](
	[id] [bigint] NOT NULL,
	[salary_accounts_rac_profiling_id] [bigint] NOT NULL,
	[month] [varchar](50) NOT NULL,
	[amount] [decimal](18, 2) NOT NULL,
	[transaction_date] [date] NULL,
	[evidence_save_path] [varchar](max) NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
	[approved_on] [datetime] NULL,
	[approved_by] [varchar](100) NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_salary_payment_history] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[security_question]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[security_question](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[question] [varchar](100) NOT NULL,
	[active_flag] [bit] NOT NULL,
 CONSTRAINT [PK_security_question] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sub_menu]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sub_menu](
	[id] [bigint] NOT NULL,
	[display_name] [varchar](50) NULL,
	[access_name] [varchar](100) NOT NULL,
	[main_menu_id] [bigint] NOT NULL,
	[url] [varchar](100) NOT NULL,
	[active_flag] [bit] NOT NULL,
	[arrangement_order] [int] NOT NULL,
	[display_flag] [bit] NOT NULL,
	[maker_page_flag] [bit] NOT NULL,
	[checker_page_id] [bigint] NULL,
 CONSTRAINT [PK_sub_menu] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[HashedPan] [varchar](500) NULL,
	[TruncatedPan] [varchar](15) NULL,
	[AppName] [varchar](50) NULL,
	[Amount] [decimal](18, 2) NULL,
	[ValueDate] [datetime] NULL,
	[STAN] [varchar](15) NULL,
	[FintechName] [varchar](10) NULL,
	[MerchantID] [varchar](15) NULL,
	[Narration] [varchar](50) NULL,
	[RRN] [varchar](15) NULL,
	[ResponseCode] [varchar](5) NULL,
	[ResponseMessage] [varchar](500) NULL,
	[SourceIPAddress] [varchar](50) NULL,
	[RequestTime] [datetime] NULL,
	[FEPResponseTime] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_access_activity]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_access_activity](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[username] [varchar](50) NOT NULL,
	[fullname] [varchar](50) NOT NULL,
	[ipaddress] [varchar](50) NOT NULL,
	[macaddress] [varchar](50) NULL,
	[access_key] [varchar](max) NULL,
	[key_expiration_date] [datetime] NULL,
	[remarks] [varchar](max) NOT NULL,
	[ignore_for_account_lock] [bit] NULL,
	[created_on] [datetime] NOT NULL,
	[logout_date] [datetime] NULL,
 CONSTRAINT [PK_user_access_activity] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_account_authentication_type]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_account_authentication_type](
	[id] [int] NOT NULL,
	[authentication_type_name] [varchar](50) NOT NULL,
	[active_flag] [bit] NOT NULL,
 CONSTRAINT [PK_user_authentication_type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_account_status]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_account_status](
	[id] [int] NOT NULL,
	[account_status] [varchar](50) NOT NULL,
 CONSTRAINT [PK_user_account_status] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[username] [varchar](50) NOT NULL,
	[authentication_type_id] [int] NOT NULL,
	[branch_user_flag] [bit] NOT NULL,
	[local_password] [varchar](50) NULL,
	[password_expiry_date] [datetime] NULL,
	[person_id] [bigint] NOT NULL,
	[role_id] [bigint] NOT NULL,
	[security_question_id] [int] NULL,
	[security_question_answer] [varchar](50) NULL,
	[status_id] [int] NOT NULL,
	[last_login_date] [datetime] NULL,
	[last_logout_date] [datetime] NULL,
	[created_on] [datetime] NOT NULL,
	[created_by] [varchar](100) NOT NULL,
	[approved_on] [datetime] NULL,
	[approved_by] [varchar](100) NULL,
	[last_modified_on] [datetime] NULL,
	[last_modified_by] [varchar](100) NULL,
	[query_string] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[employer] ADD  CONSTRAINT [DF_employer_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[fintech] ADD  CONSTRAINT [DF_fintech_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[fintech_contact_persons] ADD  CONSTRAINT [DF_fintech_contact_persons_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[fintech_onboarding_documents] ADD  CONSTRAINT [DF_fintech_onboarding_documents_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[maker_checker_log] ADD  CONSTRAINT [DF_lms_back_office_maker_checker_log_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[person] ADD  CONSTRAINT [DF_person_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[roles] ADD  CONSTRAINT [DF_lms_back_office_roles_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[salary_accounts] ADD  CONSTRAINT [DF_salary_accounts_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[salary_accounts_employers] ADD  CONSTRAINT [DF_salary_accounts_employers_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[salary_accounts_rac_profiling] ADD  CONSTRAINT [DF_salary_accounts_rac_profiling_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[salary_payment_history] ADD  CONSTRAINT [DF_salary_payment_history_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_lms_back_office_users_query_string]  DEFAULT (newid()) FOR [query_string]
GO
ALTER TABLE [dbo].[fintech]  WITH CHECK ADD  CONSTRAINT [FK_fintech_relationship_manager_person] FOREIGN KEY([relationship_manager_person_id])
REFERENCES [dbo].[person] ([id])
GO
ALTER TABLE [dbo].[fintech] CHECK CONSTRAINT [FK_fintech_relationship_manager_person]
GO
ALTER TABLE [dbo].[fintech_contact_persons]  WITH CHECK ADD  CONSTRAINT [FK_fintech_contact_persons_fintech] FOREIGN KEY([fintech_id])
REFERENCES [dbo].[fintech] ([id])
GO
ALTER TABLE [dbo].[fintech_contact_persons] CHECK CONSTRAINT [FK_fintech_contact_persons_fintech]
GO
ALTER TABLE [dbo].[fintech_contact_persons]  WITH CHECK ADD  CONSTRAINT [FK_fintech_contact_persons_person] FOREIGN KEY([person_id])
REFERENCES [dbo].[person] ([id])
GO
ALTER TABLE [dbo].[fintech_contact_persons] CHECK CONSTRAINT [FK_fintech_contact_persons_person]
GO
ALTER TABLE [dbo].[fintech_fee_operation_log]  WITH CHECK ADD  CONSTRAINT [FK_fee_operation_log_fintech] FOREIGN KEY([fintech_id])
REFERENCES [dbo].[fintech] ([id])
GO
ALTER TABLE [dbo].[fintech_fee_operation_log] CHECK CONSTRAINT [FK_fee_operation_log_fintech]
GO
ALTER TABLE [dbo].[fintech_onboarding_documents]  WITH CHECK ADD  CONSTRAINT [FK_fintech_onboarding_documents_fintech] FOREIGN KEY([fintech_id])
REFERENCES [dbo].[fintech] ([id])
GO
ALTER TABLE [dbo].[fintech_onboarding_documents] CHECK CONSTRAINT [FK_fintech_onboarding_documents_fintech]
GO
ALTER TABLE [dbo].[fintech_onboarding_documents]  WITH CHECK ADD  CONSTRAINT [FK_fintech_onboarding_documents_onboarding_documents] FOREIGN KEY([document_id])
REFERENCES [dbo].[onboarding_documents] ([id])
GO
ALTER TABLE [dbo].[fintech_onboarding_documents] CHECK CONSTRAINT [FK_fintech_onboarding_documents_onboarding_documents]
GO
ALTER TABLE [dbo].[forgot_password_log]  WITH CHECK ADD  CONSTRAINT [FK_forgot_password_log_person] FOREIGN KEY([person_id])
REFERENCES [dbo].[person] ([id])
GO
ALTER TABLE [dbo].[forgot_password_log] CHECK CONSTRAINT [FK_forgot_password_log_person]
GO
ALTER TABLE [dbo].[main_menu]  WITH CHECK ADD  CONSTRAINT [FK_main_menu_menu_icon] FOREIGN KEY([menu_icon_id])
REFERENCES [dbo].[menu_icon] ([id])
GO
ALTER TABLE [dbo].[main_menu] CHECK CONSTRAINT [FK_main_menu_menu_icon]
GO
ALTER TABLE [dbo].[password_change_log]  WITH CHECK ADD  CONSTRAINT [FK_password_change_log_person] FOREIGN KEY([person_id])
REFERENCES [dbo].[person] ([id])
GO
ALTER TABLE [dbo].[password_change_log] CHECK CONSTRAINT [FK_password_change_log_person]
GO
ALTER TABLE [dbo].[person]  WITH CHECK ADD  CONSTRAINT [FK_person_person_type] FOREIGN KEY([person_type_id])
REFERENCES [dbo].[person_type] ([id])
GO
ALTER TABLE [dbo].[person] CHECK CONSTRAINT [FK_person_person_type]
GO
ALTER TABLE [dbo].[role_menu]  WITH CHECK ADD  CONSTRAINT [FK_role_menu_roles] FOREIGN KEY([role_id])
REFERENCES [dbo].[roles] ([id])
GO
ALTER TABLE [dbo].[role_menu] CHECK CONSTRAINT [FK_role_menu_roles]
GO
ALTER TABLE [dbo].[role_menu]  WITH CHECK ADD  CONSTRAINT [FK_role_menu_sub_menu] FOREIGN KEY([menu_sub_id])
REFERENCES [dbo].[sub_menu] ([id])
GO
ALTER TABLE [dbo].[role_menu] CHECK CONSTRAINT [FK_role_menu_sub_menu]
GO
ALTER TABLE [dbo].[salary_accounts_employers]  WITH CHECK ADD  CONSTRAINT [FK_salary_accounts_employers_employer] FOREIGN KEY([employer_id])
REFERENCES [dbo].[employer] ([id])
GO
ALTER TABLE [dbo].[salary_accounts_employers] CHECK CONSTRAINT [FK_salary_accounts_employers_employer]
GO
ALTER TABLE [dbo].[salary_accounts_employers]  WITH CHECK ADD  CONSTRAINT [FK_salary_accounts_employers_salary_accounts] FOREIGN KEY([salary_account_id])
REFERENCES [dbo].[salary_accounts] ([id])
GO
ALTER TABLE [dbo].[salary_accounts_employers] CHECK CONSTRAINT [FK_salary_accounts_employers_salary_accounts]
GO
ALTER TABLE [dbo].[salary_accounts_rac_profiling]  WITH CHECK ADD  CONSTRAINT [FK_salary_accounts_rac_profiling_salary_accounts] FOREIGN KEY([salary_account_id])
REFERENCES [dbo].[salary_accounts] ([id])
GO
ALTER TABLE [dbo].[salary_accounts_rac_profiling] CHECK CONSTRAINT [FK_salary_accounts_rac_profiling_salary_accounts]
GO
ALTER TABLE [dbo].[salary_payment_history]  WITH CHECK ADD  CONSTRAINT [FK_salary_payment_history_salary_accounts_rac_profiling] FOREIGN KEY([salary_accounts_rac_profiling_id])
REFERENCES [dbo].[salary_accounts_rac_profiling] ([id])
GO
ALTER TABLE [dbo].[salary_payment_history] CHECK CONSTRAINT [FK_salary_payment_history_salary_accounts_rac_profiling]
GO
ALTER TABLE [dbo].[sub_menu]  WITH CHECK ADD  CONSTRAINT [FK_sub_menu_main_menu] FOREIGN KEY([main_menu_id])
REFERENCES [dbo].[main_menu] ([id])
GO
ALTER TABLE [dbo].[sub_menu] CHECK CONSTRAINT [FK_sub_menu_main_menu]
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD  CONSTRAINT [FK_users_person] FOREIGN KEY([person_id])
REFERENCES [dbo].[person] ([id])
GO
ALTER TABLE [dbo].[users] CHECK CONSTRAINT [FK_users_person]
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD  CONSTRAINT [FK_users_roles] FOREIGN KEY([role_id])
REFERENCES [dbo].[roles] ([id])
GO
ALTER TABLE [dbo].[users] CHECK CONSTRAINT [FK_users_roles]
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD  CONSTRAINT [FK_users_security_question] FOREIGN KEY([security_question_id])
REFERENCES [dbo].[security_question] ([id])
GO
ALTER TABLE [dbo].[users] CHECK CONSTRAINT [FK_users_security_question]
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD  CONSTRAINT [FK_users_user_account_authentication_type] FOREIGN KEY([authentication_type_id])
REFERENCES [dbo].[user_account_authentication_type] ([id])
GO
ALTER TABLE [dbo].[users] CHECK CONSTRAINT [FK_users_user_account_authentication_type]
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD  CONSTRAINT [FK_users_user_account_status] FOREIGN KEY([status_id])
REFERENCES [dbo].[user_account_status] ([id])
GO
ALTER TABLE [dbo].[users] CHECK CONSTRAINT [FK_users_user_account_status]
GO
/****** Object:  StoredProcedure [dbo].[sp_FintechContactPersonsDeleteFintechContactPersons]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[sp_FintechContactPersonsDeleteFintechContactPersons]
(
	@paramId BIGINT OUTPUT,
	@paramFintechId VarChar(50)
)

AS

SET NOCOUNT ON;
BEGIN TRANSACTION

	if exists(select * from fintech_contact_persons where fintech_id = @paramFintechId)
	begin
		
		delete 
		from 
		fintech_contact_persons 
		where
		fintech_id = @paramFintechId

		set @paramId = 1

	end
	else
	begin
		set @paramId = -100
	end


IF @@ERROR <> 0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN -1
	END
ELSE
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END

SET NOCOUNT OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_FintechContactPersonsInsertFintechContactPersons]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[sp_FintechContactPersonsInsertFintechContactPersons]
(
	@paramId BIGINT OUTPUT,
	@paramFintechId VarChar(50),
	@paramPersonId VarChar(50),
	@paramCreatedOn VarChar(50),
	@paramCreatedBy VarChar(100)
)

AS

SET NOCOUNT ON;
BEGIN TRANSACTION

	if not exists(select * from fintech_contact_persons where fintech_id = @paramFintechId and person_id = @paramPersonId)
	begin
		declare @paramFintechContactPersonId bigint
		select @paramFintechContactPersonId = max(id) from fintech_contact_persons
		set @paramFintechContactPersonId = ISNULL(@paramFintechContactPersonId + 1, 1)
		insert into fintech_contact_persons
		(
			id,
			fintech_id,
			person_id,
			created_on,
			created_by
		)
		values
		(
			@paramFintechContactPersonId,
			@paramFintechId,
			@paramPersonId,
			@paramCreatedOn,
			@paramCreatedBy
		)
		set @paramId = @paramFintechContactPersonId
	end
	else
	begin
		set @paramId = -100
	end


IF @@ERROR <> 0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN -1
	END
ELSE
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END

SET NOCOUNT OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_FintechInsertFintech]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[sp_FintechInsertFintech]
(
	@paramId BIGINT OUTPUT,
	@paramCorporateName VarChar(max),
	@paramOfficialEmailAddress VarChar(150),
	@paramHeadOfficeAddress VarChar(max),
	@paramRelationshipManagerStaffId VarChar(50),
	@paramRelationshipManagerPersonId VarChar(50),
	@paramRelationshipManagerSolId VarChar(50),
	@paramRelationshipManagerSolName VarChar(100),
	@paramRelationshipManagerSolAddress VarChar(max),
	@paramAccountNumber VarChar(50),
	@paramAccountName VarChar(max),
	@paramFinacleTermId VarChar(50),
	@paramFeeScale VarChar(50),
	@paramScaleValue decimal(18,2),
	@paramCapAmount decimal(18,2),
	@paramCreatedOn VarChar(50),
	@paramCreatedBy VarChar(100),
	@paramApprovedOn VarChar(50),
	@paramApprovedBy VarChar(100)
)

AS

SET NOCOUNT ON;
BEGIN TRANSACTION

	if not exists(select * from fintech where official_email_address = @paramOfficialEmailAddress)
	begin
		declare @paramFintechId bigint
		select @paramFintechId = max(id) from fintech
		set @paramFintechId = ISNULL(@paramFintechId + 1, 1)
		insert into fintech
		(
			id,
			corporate_name,
			official_email_address,
			head_office_address,
			relationship_manager_staff_id,
			relationship_manager_person_id,
			relationship_manager_sol_id,
			relationship_manager_sol_name,
			relationship_manager_sol_address,
			account_number,
			account_name,
			finacle_term_id,
			fee_scale,
			scale_value,
			cap_amount,
			created_on,
			created_by,
			approved_on,
			approved_by
		)
		values
		(
			@paramFintechId,
			@paramCorporateName,
			@paramOfficialEmailAddress,
			@paramHeadOfficeAddress,
			@paramRelationshipManagerStaffId,
			@paramRelationshipManagerPersonId,
			@paramRelationshipManagerSolId,
			@paramRelationshipManagerSolName,
			@paramRelationshipManagerSolAddress,
			@paramAccountNumber,
			@paramAccountName,
			@paramFinacleTermId,
			@paramFeeScale,
			@paramScaleValue,
			@paramCapAmount,
			@paramCreatedOn,
			@paramCreatedBy,
			@paramApprovedOn,
			@paramApprovedBy
		)
		set @paramId = @paramFintechId
	end
	else
	begin
		set @paramId = -100
	end


IF @@ERROR <> 0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN -1
	END
ELSE
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END

SET NOCOUNT OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_FintechOnboardingDocumentsInsertFintechOnboardingDocuments]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[sp_FintechOnboardingDocumentsInsertFintechOnboardingDocuments]
(
	@paramId BIGINT OUTPUT,
	@paramFintechId VarChar(50),
	@paramDocumentId VarChar(50),
	@paramDocumentSavePath VarChar(max),
	@paramCreatedOn VarChar(50),
	@paramCreatedBy VarChar(100),
	@paramApprovedOn VarChar(50),
	@paramApprovedBy VarChar(100)
)

AS

SET NOCOUNT ON;
BEGIN TRANSACTION

	if not exists(select * from fintech_onboarding_documents where fintech_id = @paramFintechId and document_id = @paramDocumentId)
	begin
		declare @paramFintechOnBoardingDocumentId bigint
		select @paramFintechOnBoardingDocumentId = max(id) from fintech_onboarding_documents
		set @paramFintechOnBoardingDocumentId = ISNULL(@paramFintechOnBoardingDocumentId + 1, 1)
		insert into fintech_onboarding_documents
		(
			id,
			fintech_id,
			document_id,
			document_save_path,
			created_on,
			created_by,
			approved_on,
			approved_by
		)
		values
		(
			@paramFintechOnBoardingDocumentId,
			@paramFintechId,
			@paramDocumentId,
			@paramDocumentSavePath,
			@paramCreatedOn,
			@paramCreatedBy,
			@paramApprovedOn,
			@paramApprovedBy
		)
		set @paramId = @paramFintechOnBoardingDocumentId
	end
	else
	begin
		set @paramId = -100
	end


IF @@ERROR <> 0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN -1
	END
ELSE
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END

SET NOCOUNT OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_FintechOnboardingDocumentsUpdateFintechOnboardingDocuments]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[sp_FintechOnboardingDocumentsUpdateFintechOnboardingDocuments]
(
	@paramId BIGINT OUTPUT,
	@paramFintechId VarChar(50),
	@paramDocumentId VarChar(50),
	@paramDocumentSavePath VarChar(max),
	@paramLastModifiedOn VarChar(50),
	@paramLastModifiedBy VarChar(100)
)

AS

SET NOCOUNT ON;
BEGIN TRANSACTION

	if exists(select * from fintech_onboarding_documents where fintech_id = @paramFintechId and document_id = @paramDocumentId)
	begin

		update fintech_onboarding_documents
		set
			document_save_path = @paramDocumentSavePath,
			last_modified_on = @paramLastModifiedOn,
			last_modified_by = @paramLastModifiedBy
		where
			fintech_id = @paramFintechId and document_id = @paramDocumentId

		set @paramId = 1
	end
	else
	begin
		set @paramId = -100
	end


IF @@ERROR <> 0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN -1
	END
ELSE
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END

SET NOCOUNT OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_FintechUpdateFintech]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[sp_FintechUpdateFintech]
(
	@paramId BIGINT OUTPUT,
	@paramFintechId VarChar(50),
	@paramCorporateName VarChar(max),
	@paramOfficialEmailAddress VarChar(150),
	@paramHeadOfficeAddress VarChar(max),
	@paramRelationshipManagerStaffId VarChar(50),
	@paramRelationshipManagerPersonId VarChar(50),
	@paramRelationshipManagerSolId VarChar(50),
	@paramRelationshipManagerSolName VarChar(100),
	@paramRelationshipManagerSolAddress VarChar(max),
	@paramAccountNumber VarChar(50),
	@paramAccountName VarChar(max),
	@paramFinacleTermId VarChar(50),
	@paramFeeScale VarChar(50),
	@paramScaleValue decimal(18,2),
	@paramCapAmount decimal(18,2),
	@paramLastModifiedOn VarChar(50),
	@paramLastModifiedBy VarChar(100)
)

AS

SET NOCOUNT ON;
BEGIN TRANSACTION

	if exists(select * from fintech where id = @paramFintechId)
	begin

		declare @emailCount int

		declare @existingOfficialEmailAddress varchar(100)
		select @existingOfficialEmailAddress = official_email_address from fintech where id = @paramFintechId
		if(@existingOfficialEmailAddress <> @paramOfficialEmailAddress)
		begin
			select @emailCount = count(*) from fintech where official_email_address = @paramOfficialEmailAddress
			set @emailCount = ISNULL(@emailCount, 0)
		end
		else
		begin
			set @emailCount = 0
		end

		if(@emailCount > 0)
		begin
			set @paramId = -200
		end
		else
		begin

			update fintech
			set
				corporate_name = @paramCorporateName,
				official_email_address = @paramOfficialEmailAddress,
				head_office_address = @paramHeadOfficeAddress,
				relationship_manager_staff_id = @paramRelationshipManagerStaffId,
				relationship_manager_person_id = @paramRelationshipManagerPersonId,
				relationship_manager_sol_id = @paramRelationshipManagerSolId,
				relationship_manager_sol_name = @paramRelationshipManagerSolName,
				relationship_manager_sol_address = @paramRelationshipManagerSolAddress,
				account_number = @paramAccountNumber,
				account_name = @paramAccountName,
				finacle_term_id = @paramFinacleTermId,
				fee_scale = @paramFeeScale,
				scale_value = @paramScaleValue,
				cap_amount = @paramCapAmount,
				last_modified_on = @paramLastModifiedOn,
				last_modified_by = @paramLastModifiedBy
			where 
				id = @paramFintechId

			set @paramId = @paramFintechId
		end

	end
	else
	begin
		set @paramId = -100
	end


IF @@ERROR <> 0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN -1
	END
ELSE
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END

SET NOCOUNT OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonInsertPerson]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[sp_PersonInsertPerson]
(
	@paramId BIGINT OUTPUT,
	@paramSurname VarChar(50),
	@paramFirstname VarChar(50),
	@paramMiddlename VarChar(50),
	@paramMobileNumber VarChar(50),
	@paramEmailAddress VarChar(150),
	@paramPassport VarChar(max),
	@paramSignature VarChar(500),
	@paramPersonTypeId VarChar(50),
	@paramCreatedOn VarChar(50),
	@paramCreatedBy VarChar(50)
)

AS

SET NOCOUNT ON;
BEGIN TRANSACTION

	if not exists(select * from person where email_address = @paramEmailAddress)
	begin

		declare @paramPersonId bigint
		select @paramPersonId = max(id) from person
		set @paramPersonId = ISNULL(@paramPersonId + 1, 1)
		insert into person
		(
			id,
			surname,
			first_name,
			middle_name,
			mobile_number,
			email_address,
			passport,
			signature,
			person_type_id,
			created_on,
			created_by
		)
		values
		(
			@paramPersonId,
			@paramSurname,
			@paramFirstname,
			@paramMiddlename,
			@paramMobileNumber,
			@paramEmailAddress,
			@paramPassport,
			@paramSignature,
			@paramPersonTypeId,
			@paramCreatedOn,
			@paramCreatedBy
		)

		set @paramId = @paramPersonId

	end
	else
	begin

		select @paramPersonId = id from person where email_address = @paramEmailAddress
		set @paramId = @paramPersonId

	end


IF @@ERROR <> 0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN -1
	END
ELSE
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END

SET NOCOUNT OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_PersonUpdatePerson]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[sp_PersonUpdatePerson]
(
	@paramId BIGINT OUTPUT,
	@paramSurname VarChar(50),
	@paramFirstname VarChar(50),
	@paramMiddlename VarChar(50),
	@paramMobileNumber VarChar(50),
	@paramEmailAddress VarChar(150),
	@paramPassport VarChar(max),
	@paramSignature VarChar(500),
	@paramPersonTypeId VarChar(50),
	@paramLastModifiedOn VarChar(50),
	@paramLastModifiedBy VarChar(50)
)

AS

SET NOCOUNT ON;
BEGIN TRANSACTION

	if exists(select * from person where email_address = @paramEmailAddress)
	begin
		
		select @paramId = id from person where email_address = @paramEmailAddress

		update person
		set
			surname = @paramSurname,
			first_name = @paramFirstname,
			middle_name = @paramMiddlename,
			mobile_number = @paramMobileNumber,
			email_address = @paramEmailAddress,
			passport = @paramPassport,
			signature = @paramSignature,
			person_type_id = @paramPersonTypeId,
			last_modified_on = @paramLastModifiedOn,
			last_modified_by = @paramLastModifiedBy
		where
			id = @paramId

	end
	else
	begin

		Exec sp_PersonInsertPerson
		@paramId OUTPUT,
		@paramSurname,
		@paramFirstname,
		@paramMiddlename,
		@paramMobileNumber,
		@paramEmailAddress,
		@paramPassport,
		@paramSignature,
		@paramPersonTypeId,
		@paramLastModifiedOn,
		@paramLastModifiedBy

	end

IF @@ERROR <> 0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN -1
	END
ELSE
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END

SET NOCOUNT OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_SalaryAccountInsertSalaryAccount]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[sp_SalaryAccountInsertSalaryAccount]
(
	@paramId BIGINT OUTPUT,
	@paramAccountNumber VarChar(50),
	@paramAccountName VarChar(500),
	@paramCreatedOn VarChar(50),
	@paramCreatedBy VarChar(50),
	@paramApprovedOn VarChar(50),
	@paramApprovedBy VarChar(50)
)

AS

SET NOCOUNT ON;
BEGIN TRANSACTION

	if not exists(select * from salary_accounts where account_number = @paramAccountNumber)
	begin

		declare @paramSalaryAccountId bigint

		insert into salary_accounts
		(
			account_number,
			account_name,
			created_on,
			created_by,
			approved_on,
			approved_by
		)
		values
		(
			@paramAccountNumber,
			@paramAccountName,
			@paramCreatedOn,
			@paramCreatedBy,
			@paramApprovedOn,
			@paramApprovedBy
		)

		select @paramSalaryAccountId = id from salary_accounts where account_number = @paramAccountNumber
		set @paramId = ISNULL(@paramSalaryAccountId,0)

	end
	else
	begin

		select @paramSalaryAccountId = id from salary_accounts where account_number = @paramAccountNumber
		set @paramId = @paramSalaryAccountId

	end


IF @@ERROR <> 0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN -1
	END
ELSE
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END

SET NOCOUNT OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_SalaryAccountRacProfilingInsertSalaryAccountRacProfiling]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[sp_SalaryAccountRacProfilingInsertSalaryAccountRacProfiling]
(
	@paramId BIGINT OUTPUT,
	@paramSalaryAccountId VarChar(50),
	@paramCreatedOn VarChar(50),
	@paramCreatedBy VarChar(100),
	@paramApprovedOn VarChar(50),
	@paramApprovedBy VarChar(100)
)

AS

SET NOCOUNT ON;
BEGIN TRANSACTION

	if not exists(select * from salary_accounts_rac_profiling where salary_account_id = @paramSalaryAccountId)
	begin
		declare @paramRacProfilingId bigint
		select @paramRacProfilingId = max(id) from salary_accounts_rac_profiling
		set @paramRacProfilingId = ISNULL(@paramRacProfilingId + 1, 1)
		insert into salary_accounts_rac_profiling
		(
			id,
			salary_account_id,
			created_on,
			created_by,
			approved_on,
			approved_by
		)
		values
		(
			@paramRacProfilingId,
			@paramSalaryAccountId,
			@paramCreatedOn,
			@paramCreatedBy,
			@paramApprovedOn,
			@paramApprovedBy
		)
		set @paramId = @paramRacProfilingId
	end
	else
	begin
		set @paramId = -100
	end


IF @@ERROR <> 0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN -1
	END
ELSE
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END

SET NOCOUNT OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_SalaryPaymentHistoryInsertSalaryPaymentHistory]    Script Date: 13-Jun-22 11:46:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[sp_SalaryPaymentHistoryInsertSalaryPaymentHistory]
(
	@paramId BIGINT OUTPUT,
	@paramSalaryAccountRacProfilingId VarChar(50),
	@paramMonth VarChar(50),
	@paramAmount VarChar(50),
	@paramTransactionDate VarChar(50),
	@paramEvidenceSavePath VarChar(max),
	@paramCreatedOn VarChar(50),
	@paramCreatedBy VarChar(100),
	@paramApprovedOn VarChar(50),
	@paramApprovedBy VarChar(100)
)

AS

SET NOCOUNT ON;
BEGIN TRANSACTION

	if not exists(select * from salary_payment_history where salary_accounts_rac_profiling_id = @paramSalaryAccountRacProfilingId and [month] = @paramMonth)
	begin
		declare @paramHistoryId bigint
		select @paramHistoryId = max(id) from salary_payment_history
		set @paramHistoryId = ISNULL(@paramHistoryId + 1, 1)
		insert into salary_payment_history
		(
			id,
			salary_accounts_rac_profiling_id,
			[month],
			amount,
			transaction_date,
			evidence_save_path,
			created_on,
			created_by,
			approved_on,
			approved_by
		)
		values
		(
			@paramHistoryId,
			@paramSalaryAccountRacProfilingId,
			@paramMonth,
			@paramAmount,
			@paramTransactionDate,
			@paramEvidenceSavePath,
			@paramCreatedOn,
			@paramCreatedBy,
			@paramApprovedOn,
			@paramApprovedBy
		)

		set @paramId = @paramHistoryId
	end
	else
	begin
		set @paramId = -100
	end


IF @@ERROR <> 0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN -1
	END
ELSE
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END

SET NOCOUNT OFF
GO
USE [master]
GO
ALTER DATABASE [SalaryAccountManagementPortal] SET  READ_WRITE 
GO
