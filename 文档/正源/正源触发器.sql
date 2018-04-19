/*  
Sqlserver数据库开始相关服务  
以下示例显示了如何查看 OLE Automation Procedures 的当前设置。0未启用  
*/  
EXEC sp_configure 'show advanced option', '1' --只有这个高级选项被打开的时候，才有权限修改其他配置。  
go   
RECONFIGURE     --运行RECONFIGURE语句进行安装，也就是说，使以上语句生效  
go   
EXEC sp_configure 'Ole Automation Procedures'  
GO  
  
--启用Ole Automation Procedures  
sp_configure 'show advanced options', 1  
GO  
RECONFIGURE  
GO  
sp_configure 'Ole Automation Procedures', 1  
GO  
RECONFIGURE  
GO  
  
  
---通过sql server 2008 调用应用程序    
CREATE PROCEDURE  [dbo].[pro_NotifyApp](  
    @Type NVARCHAR(20),
    @ClaseName NVARCHAR(40),
    @Detail NVARCHAR(MAX)
)       
AS    
BEGIN    
    declare @ServiceUrl as varchar(1000)     
    
    --通过http协议调用的接口地址'     
    set @ServiceUrl = 'http://localhost/ZYWebAPI/api/data/GetNotify?type=' + @Type+'&claseName='+ @ClaseName+ '&detail=' + @Detail   
    PRINT @ServiceUrl
	
    Declare @Object as Int    
    Declare @ResponseText as Varchar(8000)    
    
    Exec sp_OACreate 'MSXML2.XMLHTTP', @Object OUT    
    Exec sp_OAMethod @Object, 'open', NULL, 'get',@ServiceUrl,'false'    
    Exec sp_OAMethod @Object, 'send'    
    Exec sp_OAMethod @Object, 'responseText', @ResponseText OUTPUT    
    
    Select @ResponseText         
    Exec sp_OADestroy @Object    
END   

--物料更新
CREATE TRIGGER t_icitem_trigger ON[dbo].[t_icitem]
AFTER UPDATE
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FShortNumber NVARCHAR(50)
DECLARE @FName NVARCHAR(50)
DECLARE @FTypeID NVARCHAR(50)
DECLARE @FUnitID NVARCHAR(50)
DECLARE @FModel NVARCHAR(50)
DECLARE @FModifyTime NVARCHAR(50)
DECLARE @FItemID NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='UPDATE'
set @ClaseName='Materiel'
set @FShortNumber=(select Inserted.FShortNumber from Inserted)
set @FName=(select Inserted.FName from Inserted)
set @FTypeID=(select Inserted.FTypeID from Inserted)
set @FUnitID=(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID = (select Inserted.FUnitID from Inserted))
set @FModel=(select Inserted.FModel from Inserted)
set @FModifyTime=(select Inserted.FModifyTime from Inserted)
set @FItemID=(select Inserted.FItemID from Deleted)
set @Detail='{FShortNumber:' + @FShortNumber+' ,FName:' + @FName+',FTypeID:' +@FTypeID+',FUnitID:' +@FUnitID+',FModel:' +@FModel+',FModifyTime:' +@FModifyTime+',FItemID:' +@FItemID+'}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END  
--物料新增
CREATE TRIGGER t_icitem_trigger ON[dbo].[t_icitem]
AFTER INSERT
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FShortNumber NVARCHAR(50)
DECLARE @FName NVARCHAR(50)
DECLARE @FTypeID NVARCHAR(50)
DECLARE @FUnitID NVARCHAR(50)
DECLARE @FModel NVARCHAR(50)
DECLARE @FModifyTime NVARCHAR(50)
DECLARE @FItemID NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='INSERT'
set @ClaseName='Materiel'
set @FShortNumber=(select Inserted.FShortNumber from Inserted)
set @FName=(select Inserted.FName from Inserted)
set @FTypeID=(select Inserted.FTypeID from Inserted)
set @FUnitID=(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID = (select Inserted.FUnitID from Inserted))
set @FModel=(select Inserted.FModel from Inserted)
set @FModifyTime=(select Inserted.FModifyTime from Inserted)
set @FItemID=(select Inserted.FItemID from Deleted)
set @Detail='{FShortNumber:' + @FShortNumber+' ,FName:' + @FName+',FTypeID:' +@FTypeID+',FUnitID:' +@FUnitID+',FModel:' +@FModel+',FModifyTime:' +@FModifyTime+',FItemID:' +@FItemID+'}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END  
--物料删除
CREATE TRIGGER t_icitem_trigger ON[dbo].[t_icitem]
AFTER DELETE
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FItemID NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='DELETE'
set @ClaseName='Materiel'
set @FItemID=(select Inserted.FItemID from Deleted)
set @Detail='{FItemID:' + @FItemID+'}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END 

--生产任务单更新
CREATE TRIGGER ICmo_trigger ON[dbo].[ICmo]
AFTER UPDATE
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FBrNo NVARCHAR(50)
DECLARE @FShortNumber NVARCHAR(50)
DECLARE @FBillerID NVARCHAR(50)
DECLARE @FCheckDate NVARCHAR(50)
DECLARE @FBOMNumber NVARCHAR(50)
DECLARE @FVersion NVARCHAR(50)
DECLARE @FStatus NVARCHAR(50)
DECLARE @FAuxQty NVARCHAR(50)
DECLARE @FUnitID NVARCHAR(50)
DECLARE @FType NVARCHAR(50)
DECLARE @FPlanCommitDate NVARCHAR(50)
DECLARE @FPlanFinishDate NVARCHAR(50)
DECLARE @FWorkShop NVARCHAR(50)
DECLARE @FWorkTypeID NVARCHAR(50)
DECLARE @FConfirmDate NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='UPDATE'
set @ClaseName='ProductionPlan'
set @FBrNo=(select Inserted.FBrNo from Inserted)
set @FShortNumber=(select FShortNumber from t_icitem where t_icitem.FItemID=(select Inserted.FItemID from Inserted))
set @FBillerID=(select Inserted.FBillerID from Inserted)
set @FCheckDate=(select Inserted.FCheckDate from Inserted)
set @FBOMNumber=(select FBOMNumber from icbom where icbom.FInterID= (select Inserted.FBomInterID from Inserted))
set @FVersion=(select FVersion from icbom where icbom.FInterID= (select Inserted.FBomInterID from Inserted))
set @FStatus=(select Inserted.FStatus from Inserted)
set @FAuxQty=(select Inserted.FAuxQty from Inserted)
set @FUnitID=(select Inserted.FUnitID from Inserted)
set @FType=(select Inserted.FType from Inserted)
set @FPlanCommitDate=(select Inserted.FPlanCommitDate from Inserted)
set @FPlanFinishDate=(select Inserted.FPlanFinishDate from Inserted)
set @FWorkShop=(select Inserted.FWorkShop from Inserted)
set @FWorkTypeID=(select Inserted.FWorkTypeID from Inserted)
set @FConfirmDate=(select Inserted.FConfirmDate from Inserted)
set @Detail='{FBrNo:' + @FBrNo+',FShortNumber:' + @FShortNumber+',FBillerID:' +@FBillerID+',FCheckDate:' +@FCheckDate+',FBOMNumber:' +@FBOMNumber+',FVersion:' +@FVersion+',FStatus:' +@FStatus+ ',FAuxQty:' + @FAuxQty+',FUnitID:' +@FUnitID+',FType:' +@FType+',FPlanCommitDate:' +@FPlanCommitDate+  ',FPlanFinishDate:' + @FPlanFinishDate+',FWorkShop:' +@FWorkShop+',FWorkTypeID:' +@FWorkTypeID+',FConfirmDate:' +@FConfirmDate+'}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END  
--生产任务单新增
CREATE TRIGGER ICmo_trigger ON[dbo].[ICmo]
AFTER INSERT
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FBrNo NVARCHAR(50)
DECLARE @FShortNumber NVARCHAR(50)
DECLARE @FBillerID NVARCHAR(50)
DECLARE @FCheckDate NVARCHAR(50)
DECLARE @FBOMNumber NVARCHAR(50)
DECLARE @FVersion NVARCHAR(50)
DECLARE @FStatus NVARCHAR(50)
DECLARE @FAuxQty NVARCHAR(50)
DECLARE @FUnitID NVARCHAR(50)
DECLARE @FType NVARCHAR(50)
DECLARE @FPlanCommitDate NVARCHAR(50)
DECLARE @FPlanFinishDate NVARCHAR(50)
DECLARE @FWorkShop NVARCHAR(50)
DECLARE @FWorkTypeID NVARCHAR(50)
DECLARE @FConfirmDate NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='INSERT'
set @ClaseName='ProductionPlan'
set @FBrNo=(select Inserted.FBrNo from Inserted)
set @FShortNumber=(select FShortNumber from t_icitem where t_icitem.FItemID=(select Inserted.FItemID from Inserted))
set @FBillerID=(select Inserted.FBillerID from Inserted)
set @FCheckDate=(select Inserted.FCheckDate from Inserted)
set @FBOMNumber=(select FBOMNumber from icbom where icbom.FInterID= (select Inserted.FBomInterID from Inserted))
set @FVersion=(select FVersion from icbom where icbom.FInterID= (select Inserted.FBomInterID from Inserted))
set @FStatus=(select Inserted.FStatus from Inserted)
set @FAuxQty=(select Inserted.FAuxQty from Inserted)
set @FUnitID=(select Inserted.FUnitID from Inserted)
set @FType=(select Inserted.FType from Inserted)
set @FPlanCommitDate=(select Inserted.FPlanCommitDate from Inserted)
set @FPlanFinishDate=(select Inserted.FPlanFinishDate from Inserted)
set @FWorkShop=(select Inserted.FWorkShop from Inserted)
set @FWorkTypeID=(select Inserted.FWorkTypeID from Inserted)
set @FConfirmDate=(select Inserted.FConfirmDate from Inserted)
set @Detail='{FBrNo:' + @FBrNo+',FShortNumber:' + @FShortNumber+',FBillerID:' +@FBillerID+',FCheckDate:' +@FCheckDate+',FBOMNumber:' +@FBOMNumber+',FVersion:' +@FVersion+',FStatus:' +@FStatus+ ',FAuxQty:' + @FAuxQty+',FUnitID:' +@FUnitID+',FType:' +@FType+',FPlanCommitDate:' +@FPlanCommitDate+  ',FPlanFinishDate:' + @FPlanFinishDate+',FWorkShop:' +@FWorkShop+',FWorkTypeID:' +@FWorkTypeID+',FConfirmDate:' +@FConfirmDate+'}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END  
--生产任务单删除
CREATE TRIGGER ICmo_trigger ON[dbo].[ICmo]
AFTER DELETE
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FBillerID NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='DELETE'
set @ClaseName='ProductionPlan'
set @FBillerID=(select Inserted.FBillerID from Inserted)
set @Detail='{FBillerID:' +@FBillerID+'}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END  

--BOM主表更新
CREATE TRIGGER ICmo_trigger ON[dbo].[ICmo]
AFTER UPDATE
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FBOMNumber NVARCHAR(50)
DECLARE @fshortnumber NVARCHAR(50)
DECLARE @FVersion NVARCHAR(50)
DECLARE @FStatus NVARCHAR(50)
DECLARE @FQty NVARCHAR(50)
DECLARE @FUnitID NVARCHAR(50)
DECLARE @FInterID NVARCHAR(50)
DECLARE @FEnterTime NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='UPDATE'
set @ClaseName='BOM'
set @FBOMNumber=(select Inserted.FBOMNumber from Inserted)
set @fshortnumber=(select FShortNumber from t_icitem where t_icitem.FItemID=(select Inserted.FItemID from Inserted))
set @FVersion=(select Inserted.FVersion from Inserted)
set @FStatus=(select Inserted.FStatus from Inserted)
set @FQty=(select Inserted.FQty from Inserted)
set @FUnitID=(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=(select Inserted.FUnitID from Inserted))
set @FInterID=(select Inserted.FInterID from Inserted)
set @FEnterTime=(select Inserted.FEnterTime from Inserted)
set @Detail='{FBOMNumber:' + @FBOMNumber+',fshortnumber:' + @fshortnumber+',FVersion:' +@FVersion+',FStatus:' +@FStatus+',FQty:' +@FQty+',FUnitID:' +@FUnitID+',FInterID:' +@FInterID+ ',FEnterTime:' + @FEnterTime+'}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END  
--BOM主表新增
CREATE TRIGGER ICmo_trigger ON[dbo].[ICmo]
AFTER INSERT
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FBOMNumber NVARCHAR(50)
DECLARE @fshortnumber NVARCHAR(50)
DECLARE @FVersion NVARCHAR(50)
DECLARE @FStatus NVARCHAR(50)
DECLARE @FQty NVARCHAR(50)
DECLARE @FUnitID NVARCHAR(50)
DECLARE @FInterID NVARCHAR(50)
DECLARE @FEnterTime NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='INSERT'
set @ClaseName='BOM'
set @FBOMNumber=(select Inserted.FBOMNumber from Inserted)
set @fshortnumber=(select FShortNumber from t_icitem where t_icitem.FItemID=(select Inserted.FItemID from Inserted))
set @FVersion=(select Inserted.FVersion from Inserted)
set @FStatus=(select Inserted.FStatus from Inserted)
set @FQty=(select Inserted.FQty from Inserted)
set @FUnitID=(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=(select Inserted.FUnitID from Inserted))
set @FInterID=(select Inserted.FInterID from Inserted)
set @FEnterTime=(select Inserted.FEnterTime from Inserted)
set @Detail='{FBOMNumber:' + @FBOMNumber+',fshortnumber:' + @fshortnumber+',FVersion:' +@FVersion+',FStatus:' +@FStatus+',FQty:' +@FQty+',FUnitID:' +@FUnitID+',FInterID:' +@FInterID+ ',FEnterTime:' + @FEnterTime+'}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END  
--BOM主表删除
CREATE TRIGGER ICmo_trigger ON[dbo].[ICmo]
AFTER DELETE
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FInterID NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='DELETE'
set @ClaseName='BOM'
set @FInterID=(select Inserted.FInterID from Inserted)
set @Detail='{FInterID:' +@FInterID+ '}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END  

--BOM子表更新
CREATE TRIGGER ICBOMCHILD_trigger ON[dbo].[ICBOMCHILD]
AFTER UPDATE
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FInterID NVARCHAR(50)
DECLARE @FEntryID NVARCHAR(50)
DECLARE @detailfshortnumber NVARCHAR(50)
DECLARE @detailFUnitID NVARCHAR(50)
DECLARE @detailfqty NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='UPDATE'
set @ClaseName='BOM'
set @FInterID=(select Inserted.FInterID from Inserted)
set @FEntryID=(select Inserted.FEntryID from Inserted)
set @detailfshortnumber=(select FShortNumber from t_icitem where t_icitem.FItemID=(select Inserted.FItemID from Inserted)) 
set @detailFUnitID=(select (SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=t_icitem.FUnitID)from t_icitem where t_icitem.FItemID=(select Inserted.FItemID from Inserted))
set @detailfqty=(select Inserted.FQty from Inserted)
set @Detail='{FInterID:' + @FInterID+',FEntryID:' + @FEntryID+',detailfshortnumber:' +@detailfshortnumber+',detailFUnitID:' +@detailFUnitID+',detailfqty:' +@detailfqty+'}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END  
--BOM子表新增
CREATE TRIGGER ICBOMCHILD_trigger ON[dbo].[ICBOMCHILD]
AFTER INSERT
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FInterID NVARCHAR(50)
DECLARE @FEntryID NVARCHAR(50)
DECLARE @detailfshortnumber NVARCHAR(50)
DECLARE @detailFUnitID NVARCHAR(50)
DECLARE @detailfqty NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='INSERT'
set @ClaseName='BOM'
set @FInterID=(select Inserted.FInterID from Inserted)
set @FEntryID=(select Inserted.FEntryID from Inserted)
set @detailfshortnumber=(select FShortNumber from t_icitem where t_icitem.FItemID=(select Inserted.FItemID from Inserted)) 
set @detailFUnitID=(select (SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=t_icitem.FUnitID)from t_icitem where t_icitem.FItemID=(select Inserted.FItemID from Inserted))
set @detailfqty=(select Inserted.FQty from Inserted)
set @Detail='{FInterID:' + @FInterID+',FEntryID:' + @FEntryID+',detailfshortnumber:' +@detailfshortnumber+',detailFUnitID:' +@detailFUnitID+',detailfqty:' +@detailfqty+'}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END  
--BOM子表删除
CREATE TRIGGER ICBOMCHILD_trigger ON[dbo].[ICBOMCHILD]
AFTER DELETE
AS
BEGIN
DECLARE @Type NVARCHAR(50)
DECLARE @ClaseName NVARCHAR(50)
DECLARE @FInterID NVARCHAR(50)
DECLARE @FEntryID NVARCHAR(50)
DECLARE @Detail NVARCHAR(50)
set @Type='DELETE'
set @ClaseName='BOM'
set @FInterID=(select Inserted.FInterID from Inserted)
set @FEntryID=(select Inserted.FEntryID from Inserted)
set @Detail='{FInterID:' + @FInterID+',FEntryID:' + @FEntryID+'}'
EXEC pro_NotifyApp @Type,@ClaseName, @Detail
END  