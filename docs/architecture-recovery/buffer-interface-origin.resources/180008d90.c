
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 FUN_180008d90(longlong param_1,undefined4 param_2,undefined8 *param_3)

{
  char *lpName;
  DWORD dwMaximumSizeHigh;
  char cVar1;
  DWORD DVar2;
  int iVar3;
  DWORD DVar4;
  longlong lVar5;
  undefined8 *puVar6;
  HANDLE pvVar7;
  LPVOID pvVar8;
  undefined8 uVar9;
  DWORD dwMaximumSizeLow;
  double dVar10;
  undefined8 in_stack_ffffffffffffffa0;
  undefined4 uVar11;
  undefined8 in_stack_ffffffffffffffa8;
  undefined4 uVar12;
  
  uVar11 = (undefined4)((ulonglong)in_stack_ffffffffffffffa0 >> 0x20);
  uVar12 = (undefined4)((ulonglong)in_stack_ffffffffffffffa8 >> 0x20);
  if (param_3 != (undefined8 *)0x0) {
    if (*(uint *)(param_3 + 2) != 0) {
      lpName = (char *)((longlong)param_3 + 0x14);
      lVar5 = -1;
      do {
        lVar5 = lVar5 + 1;
      } while (lpName[lVar5] != '\0');
      if (lVar5 != 0) {
        DVar4 = 0;
        dVar10 = (double)*(uint *)(param_3 + 2) * _DAT_180013cf8 + _DAT_180013d00;
        lVar5 = 0;
        if ((DAT_180013d10 <= dVar10) && (dVar10 = dVar10 - DAT_180013d10, dVar10 < DAT_180013d10))
        {
          lVar5 = -0x8000000000000000;
        }
        lVar5 = (longlong)dVar10 + lVar5;
        dwMaximumSizeLow = (DWORD)lVar5;
        dwMaximumSizeHigh = (DWORD)((ulonglong)lVar5 >> 0x20);
        if (lVar5 == 0) {
          pvVar7 = (HANDLE)0x0;
        }
        else {
          pvVar7 = CreateFileMappingA((HANDLE)0xffffffffffffffff,(LPSECURITY_ATTRIBUTES)0x0,4,
                                      dwMaximumSizeHigh,dwMaximumSizeLow,lpName);
          DVar4 = GetLastError();
        }
        param_3[1] = pvVar7;
        *(DWORD *)(param_3 + 2) = dwMaximumSizeLow;
        if (DVar4 != 0) {
          uVar9 = 4;
          cVar1 = HSlog_ShouldLog();
          if (cVar1 != '\0') {
            puVar6 = FUN_180001310();
            if (0xf < (ulonglong)puVar6[3]) {
              puVar6 = (undefined8 *)*puVar6;
            }
            DVar2 = GetLastError();
            uVar9 = 4;
            HSlog_LogPrint(4,
                           "[%s] [ModuId:%d] [%s@%d] Create file mapping, name=%s, size=%llu, error=%d"
                           ,puVar6,param_2,"CModuleSharedMemoryBase::CreateOpenMapping",0x124,lpName
                           ,lVar5,DVar2);
          }
          FUN_1800090f0(uVar9,param_2,param_3);
          iVar3 = FUN_180009350(param_1,lpName);
          if (iVar3 == 0) {
            if (lVar5 == 0) {
              pvVar7 = (HANDLE)0x0;
            }
            else {
              pvVar7 = CreateFileMappingA((HANDLE)0xffffffffffffffff,(LPSECURITY_ATTRIBUTES)0x0,4,
                                          dwMaximumSizeHigh,dwMaximumSizeLow,lpName);
              DVar4 = GetLastError();
            }
            param_3[1] = pvVar7;
            *(DWORD *)(param_3 + 2) = dwMaximumSizeLow;
            if (DVar4 == 0) goto LAB_180008fad;
            uVar9 = 4;
            cVar1 = HSlog_ShouldLog();
            if (cVar1 != '\0') {
              puVar6 = FUN_180001310();
              if (0xf < (ulonglong)puVar6[3]) {
                puVar6 = (undefined8 *)*puVar6;
              }
              DVar4 = GetLastError();
              uVar9 = 4;
              HSlog_LogPrint(4,
                             "[%s] [ModuId:%d] [%s@%d] Create file mapping, name=%s, size=%llu, error=%d"
                             ,puVar6,param_2,"CModuleSharedMemoryBase::CreateOpenMapping",0x130,
                             lpName,lVar5,DVar4);
            }
            FUN_1800090f0(uVar9,param_2,param_3);
          }
          return 0xe0000015;
        }
LAB_180008fad:
        pvVar8 = MapViewOfFile((HANDLE)param_3[1],0xf001f,0,0,0);
        *param_3 = pvVar8;
        if (pvVar8 == (LPVOID)0x0) {
          uVar9 = 4;
          cVar1 = HSlog_ShouldLog();
          if (cVar1 != '\0') {
            puVar6 = FUN_180001310();
            if (0xf < (ulonglong)puVar6[3]) {
              puVar6 = (undefined8 *)*puVar6;
            }
            DVar4 = GetLastError();
            uVar9 = 4;
            HSlog_LogPrint(4,
                           "[%s] [ModuId:%d] [%s@%d] Map view of file failed, name=%s, sys error=%d"
                           ,puVar6,param_2,"CModuleSharedMemoryBase::CreateOpenMapping",0x13a,lpName
                           ,DVar4);
          }
          FUN_1800090f0(uVar9,param_2,param_3);
          return 0xe0000015;
        }
        return 0;
      }
    }
    cVar1 = HSlog_ShouldLog(4);
    if (cVar1 != '\0') {
      if ((DAT_180019b48 & 1) == 0) {
        DAT_180019b48 = DAT_180019b48 | 1;
        FUN_180001350();
        atexit(FUN_1800111b0);
      }
      puVar6 = &DAT_180019b28;
      if (0xf < DAT_180019b40) {
        puVar6 = DAT_180019b28;
      }
      HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] CreateOpenMapping fail,size=%d, len=%d",puVar6,
                     param_2,"CModuleSharedMemoryBase::CreateOpenMapping",CONCAT44(uVar11,0x119),
                     CONCAT44(uVar12,*(undefined4 *)(param_3 + 2)),(longlong)param_3 + 0x14);
    }
  }
  return 0xe0000001;
}

