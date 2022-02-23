use [Nguoicocong]

alter table quytrinh_lichsu 
add checkers varchar(500)

alter table quytrinh_lichsu 
add nguoi_nhan varchar(500)

--update checkers
UPDATE
    quytrinh_lichsu 
SET
    quytrinh_lichsu.checkers = RAN.checkers
FROM
    quytrinh_lichsu SI
INNER JOIN
    quytrinh_quatrinhduyet RAN
ON 
    SI.id_quatrinh = RAN.id_row and RAN.checkers is not null;

	
UPDATE
    quytrinh_lichsu 
SET
    quytrinh_lichsu.checkers = RAN.checker
FROM
    quytrinh_lichsu SI
INNER JOIN
    quytrinh_quatrinhduyet RAN
ON 
    SI.id_quatrinh = RAN.id_row and SI.checkers is null;

	--update người nhận
UPDATE
    quytrinh_lichsu 
SET
    quytrinh_lichsu.nguoi_nhan = RAN.checkers
FROM
    quytrinh_lichsu SI
inner join quytrinh_quatrinhduyet_next qt on SI.id_quatrinh=qt.id_row
INNER JOIN    quytrinh_quatrinhduyet RAN ON     qt.id_quatrinh_next = RAN.id_row 
where RAN.checkers is not null;

UPDATE
    quytrinh_lichsu 
SET
    quytrinh_lichsu.nguoi_nhan = RAN.checker
FROM
    quytrinh_lichsu SI
inner join quytrinh_quatrinhduyet_next qt on SI.id_quatrinh=qt.id_row
INNER JOIN    quytrinh_quatrinhduyet RAN ON     qt.id_quatrinh_next = RAN.id_row 
	where SI.nguoi_nhan is null;

--update is_begin