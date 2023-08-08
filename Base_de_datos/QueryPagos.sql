Create database pagos

use pagos

CREATE TABLE pago(
	IdPago int identity(1,1) primary key,
	pago varchar(100),
	fecha date,
	importe int
)

ALTER TABLE pago
ADD CONSTRAINT UK_Pago UNIQUE (pago);

Insert Into pago(pago,fecha,importe) values 
('Iphone 15','2023-08-02',15000),
('Telmex','2023-08-02',500)

SELECT * FROM pago

create procedure p_Registrar(
@pago varchar(100),
@fecha date,
@importe int
)
as
begin
insert into pago(pago,fecha,importe) values (@pago,@fecha,@importe)
end

create procedure p_Editar (
@IdPago int,
@pago varchar(100),
@fecha date,
@importe int
)
as
begin
	update pago set pago = @pago, fecha = @fecha, importe = @importe where IdPago = @IdPago
end 

create procedure p_Borrar (
@IdPago int
)
as
begin
	delete from pago where IdPago = @IdPago
end 

