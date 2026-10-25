
DWORD FUN_1800090f0(undefined8 param_1,undefined4 param_2,undefined8 *param_3)

{
  char cVar1;
  BOOL BVar2;
  DWORD DVar3;
  DWORD DVar4;
  undefined8 *puVar5;
  
  if (param_3 == (undefined8 *)0x0) {
    return 0xe0000001;
  }
  DVar4 = 0;
  if (*(int *)(param_3 + 2) == 0) {
    return 0xe0000001;
  }
  DVar3 = 0;
  if ((LPCVOID)*param_3 != (LPCVOID)0x0) {
    BVar2 = UnmapViewOfFile((LPCVOID)*param_3);
    DVar3 = 0;
    if (BVar2 == 0) {
      DVar3 = GetLastError();
    }
    if (DVar3 != 0) {
      cVar1 = HSlog_ShouldLog(4);
      if (cVar1 != '\0') {
        puVar5 = FUN_180001310();
        if (0xf < (ulonglong)puVar5[3]) {
          puVar5 = (undefined8 *)*puVar5;
        }
        HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] Unmap view of file failed, name=%s, sys error=%d"
                       ,puVar5,param_2,"CModuleSharedMemoryBase::CloseMapping",0x155,
                       (longlong)param_3 + 0x14,DVar3);
      }
    }
  }
  if ((HANDLE)param_3[1] != (HANDLE)0x0) {
    BVar2 = CloseHandle((HANDLE)param_3[1]);
    if (BVar2 == 0) {
      DVar4 = GetLastError();
    }
    if (DVar4 != 0) {
      cVar1 = HSlog_ShouldLog(4);
      if (cVar1 != '\0') {
        puVar5 = FUN_180001310();
        if (0xf < (ulonglong)puVar5[3]) {
          puVar5 = (undefined8 *)*puVar5;
        }
        HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] Close file mapping failed, name=%s, sys error=%d"
                       ,puVar5,param_2,"CModuleSharedMemoryBase::CloseMapping",0x15d,
                       (longlong)param_3 + 0x14,DVar4);
      }
    }
  }
  param_3[1] = 0;
  if (DVar4 == 0) {
    DVar4 = DVar3;
  }
  *param_3 = 0;
  *(undefined4 *)(param_3 + 2) = 0;
  return DVar4;
}

