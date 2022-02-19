USE master;

IF NOT EXISTS ( SELECT * FROM sys.databases WHERE name = 'LocalisationDb' )
BEGIN
    CREATE DATABASE [LocalisationDb]
END 

USE [LocalisationDb];

IF NOT EXISTS ( SELECT * FROM sysobjects WHERE xtype = 'U' AND name = 'Language' )
BEGIN
    CREATE TABLE Language (
        LanguageId  INT     PRIMARY KEY     IDENTITY ( 1, 1 )
      , Name        VARCHAR( 50 )
      , CreatedBy   INT     NOT NULL
      , CreatedOn   DATE    NOT NULL
      , UpdatedBy   INT     NOT NULL
      , UpdatedOn   DATE    NOT NULL 
    )
END