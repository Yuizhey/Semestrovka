create table LordfilmDB.dbo.MovieDetails (
  Id int primary key not null,
  MovieDescription nvarchar(max),
  Country nvarchar(100) not null,
  ProducerId int not null,
  EngTitle nvarchar(255),
  MovieId int not null,
  Quality varchar(20),
  VideoURL varchar(250),
  foreign key (ProducerId) references Producer (Id),
  foreign key (MovieId) references Movies (Id)
);
GO

create table LordfilmDB.dbo.Movies (
  Id int primary key not null,
  RuTitle nvarchar(255) not null,
  ReleaseYear int not null,
  ImageSource nvarchar(255),
  Status nvarchar(50)
);
GO

create table LordfilmDB.dbo.MovieStatistic (
  Id int primary key not null,
  KP_Rating decimal(3,1) not null,
  IMDB_Rating decimal(3,1) not null,
  Likes_Count int not null,
  Dislikes_Count int not null,
  Movie_Id int not null,
  foreign key (Movie_Id) references Movies (Id)
);
GO

create table LordfilmDB.dbo.Producer (
  Id int primary key not null,
  Name nvarchar(255) not null,
  DirectedFilmsCount int default ((0)) not null,
  BirthYear int,
  BirthCountry nvarchar(100)
);
GO

create table LordfilmDB.dbo.Users (
  Id int primary key not null,
  Email nvarchar(255) not null,
  Login nvarchar(100) not null,
  Password nvarchar(255) not null
);
create unique index UQ__users__A9D10534349924A6 on Users (Email);
GO

