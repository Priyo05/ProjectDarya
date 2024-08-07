create table BarangRequests (
    Id int identity(1,1) primary key,
    NamaDivisi varchar(100) not null,
    KodeBarang int not null,
    NamaBarang varchar(100) not null,
    Jumlah int not null,
    Status varchar(50) default 'Pending' not null,
    RequestDate datetime default GETDATE() not null,
    foreign key (KodeBarang) references MasterBarang(KodeBarang)
);

create or alter procedure SpGetPendingRequests
as
begin
    select
		Id,
        KodeBarang, 
        NamaDivisi, 
        NamaBarang, 
        Jumlah, 
        Status, 
        RequestDate
    from BarangRequests
    where Status = 'Pending';
end