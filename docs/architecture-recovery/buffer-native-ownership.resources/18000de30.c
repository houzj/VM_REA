
/* WARNING: Function: __security_check_cookie replaced with injection: security_check_cookie */
/* WARNING: Type propagation algorithm not settling */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */
/* public: virtual int __cdecl CImageBufferModule::Process(void * __ptr64,void * __ptr64) __ptr64 */

int __thiscall CImageBufferModule::Process(CImageBufferModule *this,void *param_1,void *param_2)

{
  undefined8 uVar1;
  longlong *plVar2;
  longlong *plVar3;
  code *pcVar4;
  double dVar5;
  undefined8 uVar6;
  undefined8 uVar7;
  undefined8 uVar8;
  undefined8 uVar9;
  undefined8 uVar10;
  undefined8 uVar11;
  char cVar12;
  int iVar13;
  undefined8 *puVar14;
  longlong lVar15;
  CImageBufferModule *pCVar16;
  char *pcVar17;
  longlong *plVar18;
  ulonglong uVar19;
  ulonglong uVar20;
  undefined1 auStack_478 [32];
  int *local_458;
  undefined4 local_450;
  int local_448;
  int local_438;
  int local_434 [3];
  LARGE_INTEGER local_428;
  LARGE_INTEGER local_420;
  undefined1 local_418 [12];
  undefined4 local_40c;
  undefined4 uStack_408;
  undefined4 local_404;
  undefined4 uStack_400;
  undefined4 local_3fc;
  undefined4 uStack_3f8;
  undefined8 local_3f4;
  undefined8 local_3ec;
  undefined4 local_3e4;
  undefined8 local_3e0;
  undefined8 local_3d8;
  undefined8 uStack_3d0;
  undefined8 local_3c8;
  undefined8 uStack_3c0;
  undefined8 local_3b8;
  undefined8 uStack_3b0;
  undefined8 local_3a8;
  undefined8 local_398;
  undefined4 uStack_390;
  undefined4 local_38c;
  undefined4 uStack_388;
  undefined4 local_384;
  undefined4 uStack_380;
  undefined4 local_37c;
  undefined4 uStack_378;
  undefined8 local_374;
  undefined8 local_36c;
  undefined4 local_364;
  float local_358;
  undefined1 local_354 [684];
  char local_a8 [24];
  ulonglong local_90;
  
                    /* 0xde30  15  ?Process@CImageBufferModule@@UEAAHPEAX0@Z */
  local_3e0 = 0xfffffffffffffffe;
  local_90 = DAT_180019000 ^ (ulonglong)auStack_478;
  local_434[1] = 0;
  local_434[0] = 1;
  local_438 = 0xf;
  QueryPerformanceCounter(&local_428);
  QueryPerformanceFrequency(&local_420);
  dVar5 = ((double)local_428.QuadPart / (double)local_420.QuadPart) * _DAT_180013d08;
  local_458 = local_434 + 1;
  VM_M_GetInt(param_1,"BufferCount",0,&local_438);
  if (local_438 < 0) {
    cVar12 = HSlog_ShouldLog(4);
    if (cVar12 == '\0') {
      return -0x1ffffce4;
    }
    if ((DAT_180019b48 & 1) == 0) {
      DAT_180019b48 = DAT_180019b48 | 1;
      FUN_180001350();
      atexit(FUN_1800111b0);
    }
    local_450 = 0x81;
LAB_18000df4e:
    local_448 = 0xe000031c;
    puVar14 = &DAT_180019b28;
    if (0xf < DAT_180019b40) {
      puVar14 = DAT_180019b28;
    }
    local_458 = (int *)0x180013970;
    HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] error nRet = %#X\r\n",puVar14,
                   *(undefined4 *)(this + 0x48));
    return -0x1ffffce4;
  }
  if (0xf < local_438) {
    cVar12 = HSlog_ShouldLog(4);
    if (cVar12 == '\0') {
      return -0x1ffffce4;
    }
    if ((DAT_180019b48 & 1) == 0) {
      DAT_180019b48 = DAT_180019b48 | 1;
      FUN_180001350();
      atexit(FUN_1800111b0);
    }
    local_450 = 0x82;
    goto LAB_18000df4e;
  }
  local_458 = local_434 + 1;
  VM_M_GetInt(param_1,"RefreshFlag",0,local_434);
  if (local_434[0] == 1) {
    local_418._0_4_ = 0;
    local_418._4_8_ = 0;
    local_40c = 0;
    uStack_408 = 0;
    local_404 = 0;
    uStack_400 = 0;
    local_3fc = 0;
    uStack_3f8 = 0;
    local_3f4 = 0;
    local_3ec = 0;
    local_3e4 = 0;
    iVar13 = GetInImage(this,param_1,(ImageData *)local_418);
    if (iVar13 != 0) {
      cVar12 = HSlog_ShouldLog(4);
      if (cVar12 == '\0') {
        return iVar13;
      }
      puVar14 = FUN_180001310();
      if (0xf < (ulonglong)puVar14[3]) {
        puVar14 = (undefined8 *)*puVar14;
      }
      local_450 = 0x89;
      local_448 = iVar13;
      goto LAB_18000e075;
    }
    local_398._0_4_ = 0;
    local_398._4_4_ = 0;
    uStack_390 = 0;
    local_38c = 0;
    uStack_388 = 0;
    local_384 = 0;
    uStack_380 = 0;
    local_37c = 0;
    uStack_378 = 0;
    local_374 = 0;
    local_36c = 0;
    local_364 = 0;
    pCVar16 = this;
    iVar13 = CopyImage(this,(ImageData *)&local_398,(ImageData *)local_418);
    if (iVar13 != 0) {
      cVar12 = HSlog_ShouldLog(4);
      if (cVar12 == '\0') {
        return iVar13;
      }
      puVar14 = FUN_180001310();
      if (0xf < (ulonglong)puVar14[3]) {
        puVar14 = (undefined8 *)*puVar14;
      }
      local_450 = 0x8d;
      local_448 = iVar13;
      goto LAB_18000e075;
    }
    puVar14 = *(undefined8 **)(this + 0xd0);
    lVar15 = FUN_18000f630(pCVar16,puVar14,(undefined8 *)puVar14[1],&local_398);
    if (*(longlong *)(this + 0xd8) == 0x38e38e38e38e38d) {
      std::_Xlength_error("list<T> too long");
      pcVar4 = (code *)swi(3);
      iVar13 = (*pcVar4)();
      return iVar13;
    }
    *(longlong *)(this + 0xd8) = *(longlong *)(this + 0xd8) + 1;
    puVar14[1] = lVar15;
    **(longlong **)(lVar15 + 8) = lVar15;
    if ((ulonglong)(longlong)local_438 < *(ulonglong *)(this + 0xd8)) {
      do {
        plVar2 = (longlong *)**(undefined8 **)(this + 0xd0);
        *(longlong *)plVar2[1] = *plVar2;
        *(longlong *)(*plVar2 + 8) = plVar2[1];
        *(longlong *)(this + 0xd8) = *(longlong *)(this + 0xd8) + -1;
        operator_delete(plVar2);
      } while ((ulonglong)(longlong)local_438 < *(ulonglong *)(this + 0xd8));
    }
  }
  plVar2 = (longlong *)**(longlong **)(this + 0xd0);
  plVar18 = plVar2;
  if (plVar2 != *(longlong **)(this + 0xd0)) {
    do {
      uVar6 = plVar18[2];
      uVar7 = plVar18[3];
      uVar8 = plVar18[4];
      uVar9 = plVar18[5];
      uVar10 = plVar18[6];
      uVar11 = plVar18[7];
      uVar1 = plVar18[8];
      local_a8[0] = '\0';
      local_a8[1] = '\0';
      local_a8[2] = '\0';
      local_a8[3] = '\0';
      local_a8[4] = '\0';
      local_a8[5] = '\0';
      local_a8[6] = '\0';
      local_a8[7] = '\0';
      local_a8[8] = '\0';
      local_a8[9] = '\0';
      local_a8[10] = '\0';
      local_a8[0xb] = '\0';
      local_a8[0xc] = '\0';
      local_a8[0xd] = '\0';
      local_a8[0xe] = '\0';
      local_a8[0xf] = '\0';
      local_a8[0x10] = '\0';
      local_a8[0x11] = '\0';
      local_a8[0x12] = '\0';
      local_a8[0x13] = 0;
      for (plVar3 = plVar2; plVar3 != plVar18; plVar3 = (longlong *)*plVar3) {
      }
      sprintf_s(local_a8,0x14,"%lld");
      uStack_380 = 0xf;
      local_37c = 0;
      uStack_388 = 0;
      local_384 = 0;
      local_398._0_4_ = (uint)local_398 & 0xffffff00;
      FUN_180001f20(&local_398,"OutImage",8);
      uVar20 = 0;
      if (local_a8[0] != '\0') {
        uVar19 = 0xffffffffffffffff;
        do {
          lVar15 = uVar19 + 1;
          uVar20 = uVar19 + 1;
          uVar19 = uVar19 + 1;
        } while (local_a8[lVar15] != '\0');
      }
      FUN_180002500(&local_398,local_a8,uVar20);
      pcVar17 = (char *)&local_398;
      if (0xf < CONCAT44(local_37c,uStack_380)) {
        pcVar17 = (char *)CONCAT44(local_398._4_4_,(uint)local_398);
      }
      uStack_400 = 0xf;
      local_3fc = 0;
      uStack_408 = 0;
      local_404 = 0;
      local_418._0_4_ = local_418._0_4_ & 0xffffff00;
      uVar20 = 0;
      if (*pcVar17 != '\0') {
        uVar20 = 0xffffffffffffffff;
        do {
          uVar20 = uVar20 + 1;
        } while (pcVar17[uVar20] != '\0');
      }
      FUN_180001f20((ulonglong *)local_418,pcVar17,uVar20);
      local_3d8 = uVar6;
      uStack_3d0 = uVar7;
      local_3c8 = uVar8;
      uStack_3c0 = uVar9;
      local_3b8 = uVar10;
      uStack_3b0 = uVar11;
      local_3a8 = uVar1;
      iVar13 = SetOutImage(this,param_2,&local_3d8,local_418);
      if (iVar13 != 0) {
        cVar12 = HSlog_ShouldLog(4);
        if (cVar12 != '\0') {
          if ((DAT_180019b48 & 1) == 0) {
            DAT_180019b48 = DAT_180019b48 | 1;
            FUN_180001350();
            atexit(FUN_1800111b0);
          }
          puVar14 = &DAT_180019b28;
          if (0xf < DAT_180019b40) {
            puVar14 = DAT_180019b28;
          }
          local_450 = 0xa4;
          local_458 = (int *)0x180013970;
          local_448 = iVar13;
          HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] error nRet = %#X\r\n",puVar14,
                         *(undefined4 *)(this + 0x48));
        }
        if (0xf < CONCAT44(local_37c,uStack_380)) {
          operator_delete((void *)CONCAT44(local_398._4_4_,(uint)local_398));
          return iVar13;
        }
        return iVar13;
      }
      if (0xf < CONCAT44(local_37c,uStack_380)) {
        operator_delete((void *)CONCAT44(local_398._4_4_,(uint)local_398));
      }
      plVar18 = (longlong *)*plVar18;
    } while (plVar18 != *(undefined8 **)(this + 0xd0));
  }
  iVar13 = VM_M_SetInt(param_2,"ModuStatus",0,1);
  if (iVar13 == 0) {
    FUN_18000ccc0((longlong)this,param_1,param_2);
    QueryPerformanceCounter(&local_420);
    QueryPerformanceFrequency(&local_428);
    memset(local_354,0,0x2ac);
    local_358 = (float)(((double)local_420.QuadPart / (double)local_428.QuadPart) * _DAT_180013d08 -
                       dVar5);
    iVar13 = VM_M_SetModuleRuntimeInfo(*(undefined8 *)(this + 8),&local_358);
    if (iVar13 == 0) {
      return 0;
    }
    cVar12 = HSlog_ShouldLog(4);
    if (cVar12 == '\0') {
      return iVar13;
    }
    puVar14 = FUN_180001310();
    if (0xf < (ulonglong)puVar14[3]) {
      puVar14 = (undefined8 *)*puVar14;
    }
    local_450 = 0xb1;
    local_448 = iVar13;
  }
  else {
    cVar12 = HSlog_ShouldLog(4);
    if (cVar12 == '\0') {
      return iVar13;
    }
    if ((DAT_180019b48 & 1) == 0) {
      DAT_180019b48 = DAT_180019b48 | 1;
      FUN_180001350();
      atexit(FUN_1800111b0);
    }
    puVar14 = &DAT_180019b28;
    if (0xf < DAT_180019b40) {
      puVar14 = DAT_180019b28;
    }
    local_450 = 0xa8;
    local_448 = iVar13;
  }
LAB_18000e075:
  iVar13 = local_448;
  local_458 = (int *)0x180013970;
  HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] error nRet = %#X\r\n",puVar14,
                 *(undefined4 *)(this + 0x48));
  return iVar13;
}

