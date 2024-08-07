Create database Project

use Project

create table MasterBarang (
    KodeBarang int identity(1,1) NOT NULL PRIMARY KEY,
    NamaBarang varchar(100) NOT NULL,
    NamaSupplier varchar(100) NOT NULL,
    Harga money default 0,
    StokBarang int default 0
);

create or alter procedure SpInsertBarang
(
    @NamaBarang varchar(100),
    @NamaSupplier varchar(100),
    @Harga money = null,
    @StokBarang int = 0
)
as
begin
    insert into MasterBarang (NamaBarang, NamaSupplier, Harga, StokBarang)
    values (@NamaBarang, @NamaSupplier, @Harga, @StokBarang)
end


create or alter procedure SpSelectAllBarang
as
begin
    select KodeBarang, NamaBarang, NamaSupplier, Harga, StokBarang
    from MasterBarang
end

create or alter procedure SpSelectBarangByKode
(
    @KodeBarang int
)
as
begin
    select KodeBarang, NamaBarang, NamaSupplier, Harga, StokBarang
    from MasterBarang
    where KodeBarang = @KodeBarang
end

create or alter procedure SpUpdateBarang
(
    @KodeBarang int,
    @NamaBarang varchar(100),
    @NamaSupplier varchar(100),
    @Harga money,
    @StokBarang int
)
as
begin
    update MasterBarang
    set 
        NamaBarang = @NamaBarang,
        NamaSupplier = @NamaSupplier,
        Harga = @Harga,
        StokBarang = @StokBarang
    where KodeBarang = @KodeBarang
end

create or alter procedure SpDeleteBarang
(
    @KodeBarang int
)
as
begin
    delete from MasterBarang
    where KodeBarang = @KodeBarang
end
