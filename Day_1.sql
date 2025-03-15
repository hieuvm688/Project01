--Save file with mdf, ldf
CREATE DATABASE IOTestSetup
ON (NAME = 'IOTestSetup_data', FILENAME = 'C:\SQLData\IOTestSetup.mdf', SIZE = 10MB)
LOG ON (NAME = 'IOTestSetup_log', FILENAME = 'D:\SQLLog\IOTestSetup.ldf', SIZE = 5MB);

-- Lệnh 1 
CREATE TABLE BufferTest (ID INT, Data CHAR(8000));
INSERT INTO BufferTest SELECT TOP 1000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)), 'X' FROM sys.objects;
SELECT * FROM BufferTest;

--Xem page trong buffer pool 
SELECT COUNT(*) AS buffered_pages, DB_NAME(database_id) AS BufferTest
FROM sys.dm_os_buffer_descriptors
WHERE database_id = DB_ID('tempdb')
GROUP BY database_id;

--Lệnh 2
Exec sp_configure 'show advanced options' , 1;

RECONFIGURE

exec sp_configure 'affinity mask' , 1;

RECONFIGURE

shutdown with nowait;

select scheduler_id, cpu_id, status, is_online from sys.dm_os_schedulers

///////////////////////////////
select cpu_count from sys.dm_os_sys_info

Exec sp_configure 'affinity mask' , 255;
RECONFIGURE

--Lệnh 3
select getdate();
select top(10000) * from large_table order by id -- Xem số lượng instance đang chạy
select getdate();

--Lệnh 4
select getdate();
select top(1000000) * from large_table order by id;
select getdate();

select * from sys.dm_os_schedulers -- Xem số lượng instance đang chạy

--Lệnh 5
select * from sys.dm_os_memory_clerks;

select * from sys.dm_os_memory_node_access_stats;

select * from sys.dm_exec_requests; --Xem cá truy vấn đang được thực thi

select * from sys.dm_os_schedulers;

select * from sys.dm_os_workers;

--Lệnh 6
Exec sp_configure 'show advanced options' , 1;

RECONFIGURE

exec sp_configure 'affinity mask' , 1;

RECONFIGURE

shutdown with nowait;

select scheduler_id, cpu_id, status, is_online from sys.dm_os_schedulers

///////////////////////////////
select cpu_count from sys.dm_os_sys_info

Exec sp_configure 'affinity mask' , 255;
RECONFIGURE

--Lệnh 7
--tao table don
create table large_table ( id int identity(1,1) , data varchar(100));

insert into large_table(data) select top 1000000000 'sample data' from sys.all_columns ac1, sys.all_columns ac2;

create index idx_id on large_table( id) 

set statistics xml on;

--Lệnh 8
begin transaction;
update TestLock set Value = 190 where ID=1;
waitfor delay  '00:00:30';
Commit;

--Lệnh 9

select scheduler_id, work_queue_count, active_workers_count from sys.dm_os_schedulers where status = 'VISIBLE ONLINE';

--Lệnh 10

select blocking_session_id,wait_type from sys.dm_exec_requests;

select count(*) from sys.objects a Cross Join sys.objects b;

CREATE TABLE TestLock (ID INT PRIMARY KEY, Value INT);
INSERT INTO TestLock VALUES (1, 100), (2, 200);