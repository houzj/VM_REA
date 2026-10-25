
void FUN_180027080(longlong param_1,undefined8 *param_2)

{
  byte bVar1;
  undefined1 auVar2 [16];
  undefined1 uVar3;
  char cVar4;
  int iVar5;
  uint uVar6;
  longlong lVar7;
  byte *pbVar8;
  longlong lVar9;
  _String_val<std::_Simple_types<char>_> *p_Var10;
  undefined8 uVar11;
  __uint64 _Var12;
  void *pvVar13;
  longlong lVar14;
  double dVar15;
  int local_a4;
  ulonglong local_90;
  ulonglong local_88;
  longlong local_80;
  
  lVar7 = FUN_180009510(param_2);
  if (*(int *)(lVar7 + 0x450) != 0) {
    uVar3 = VM_M_SetGlobalVarLockStatus
                      (*(undefined4 *)(param_1 + 0x48),*(undefined4 *)(param_1 + 0x48),1);
    lVar7 = FUN_180009510(param_2);
    iVar5 = *(int *)(lVar7 + 0x450);
    lVar7 = FUN_180009510(param_2);
    pbVar8 = (byte *)(lVar7 + 0x129);
    lVar7 = (longlong)&DAT_1800579bc - (longlong)pbVar8;
    do {
      bVar1 = *pbVar8;
      if (bVar1 != pbVar8[lVar7]) {
        uVar6 = -(uint)(bVar1 < pbVar8[lVar7]) | 1;
        goto LAB_18002712e;
      }
      pbVar8 = pbVar8 + 1;
    } while (bVar1 != 0);
    uVar6 = 0;
LAB_18002712e:
    if (uVar6 == 0) {
      lVar7 = FUN_180009510(param_2);
      lVar9 = FUN_180009510(param_2);
      iVar5 = VM_M_BatchSetGlobalVarInt
                        (*(undefined8 *)(param_1 + 8),lVar9 + 0x29,*(undefined8 *)(lVar7 + 0x458),
                         iVar5,uVar3);
      if ((iVar5 != 0) && (cVar4 = HSlog_ShouldLog(4), cVar4 != '\0')) {
        p_Var10 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
        uVar11 = FUN_180003ec0(p_Var10);
        HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] VM_M_BatchSetGlobalVarInt is error:[%x]",uVar11,
                       *(undefined4 *)(param_1 + 0x48),"CVariableModuBase::SetGlobalValue",0x617,
                       iVar5);
      }
    }
    else {
      lVar7 = FUN_180009510(param_2);
      pbVar8 = (byte *)(lVar7 + 0x129);
      lVar7 = (longlong)"float" - (longlong)pbVar8;
      do {
        bVar1 = *pbVar8;
        if (bVar1 != pbVar8[lVar7]) {
          uVar6 = -(uint)(bVar1 < pbVar8[lVar7]) | 1;
          goto LAB_180027229;
        }
        pbVar8 = pbVar8 + 1;
      } while (bVar1 != 0);
      uVar6 = 0;
LAB_180027229:
      if (uVar6 == 0) {
        lVar7 = FUN_180009510(param_2);
        lVar9 = FUN_180009510(param_2);
        iVar5 = VM_M_BatchSetGlobalVarFloat
                          (*(undefined8 *)(param_1 + 8),lVar9 + 0x29,*(undefined8 *)(lVar7 + 0x460),
                           iVar5,uVar3);
        if ((iVar5 != 0) && (cVar4 = HSlog_ShouldLog(4), cVar4 != '\0')) {
          p_Var10 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
          uVar11 = FUN_180003ec0(p_Var10);
          HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] VM_M_BatchSetGlobalVarFloat is error:[%x]",
                         uVar11,*(undefined4 *)(param_1 + 0x48),"CVariableModuBase::SetGlobalValue",
                         0x61e,iVar5);
        }
      }
      else {
        lVar7 = FUN_180009510(param_2);
        pbVar8 = (byte *)(lVar7 + 0x129);
        lVar7 = (longlong)"string" - (longlong)pbVar8;
        do {
          bVar1 = *pbVar8;
          if (bVar1 != pbVar8[lVar7]) {
            uVar6 = -(uint)(bVar1 < pbVar8[lVar7]) | 1;
            goto LAB_18002732a;
          }
          pbVar8 = pbVar8 + 1;
        } while (bVar1 != 0);
        uVar6 = 0;
LAB_18002732a:
        if ((uVar6 == 0) && (lVar7 = FUN_180009510(param_2), *(longlong *)(lVar7 + 0x468) != 0)) {
          auVar2._8_8_ = 0;
          auVar2._0_8_ = (longlong)iVar5;
          _Var12 = SUB168(ZEXT816(0x10) * auVar2,0);
          if (SUB168(ZEXT816(0x10) * auVar2,8) != 0) {
            _Var12 = 0xffffffffffffffff;
          }
          pvVar13 = (void *)thunk_FUN_1800502a4(_Var12);
          if (pvVar13 == (void *)0x0) {
            cVar4 = HSlog_ShouldLog(4);
            if (cVar4 != '\0') {
              p_Var10 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
              uVar11 = FUN_180003ec0(p_Var10);
              HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] STRING_UNIT new memory is error",uVar11,
                             *(undefined4 *)(param_1 + 0x48),"CVariableModuBase::SetGlobalValue",
                             0x628);
            }
            FUN_180016480((longlong)param_2);
            return;
          }
          for (local_a4 = 0; local_a4 < iVar5; local_a4 = local_a4 + 1) {
            lVar7 = FUN_180009510(param_2);
            *(undefined8 *)((longlong)pvVar13 + (longlong)local_a4 * 0x10) =
                 *(undefined8 *)(*(longlong *)(lVar7 + 0x468) + (longlong)local_a4 * 0x60);
            lVar7 = FUN_180009510(param_2);
            *(undefined4 *)((longlong)pvVar13 + (longlong)local_a4 * 0x10 + 8) =
                 *(undefined4 *)(*(longlong *)(lVar7 + 0x468) + 8 + (longlong)local_a4 * 0x60);
            lVar7 = FUN_180009510(param_2);
            *(undefined4 *)((longlong)pvVar13 + (longlong)local_a4 * 0x10 + 0xc) =
                 *(undefined4 *)(*(longlong *)(lVar7 + 0x468) + 0xc + (longlong)local_a4 * 0x60);
          }
          lVar7 = FUN_180009510(param_2);
          iVar5 = VM_M_BatchSetGlobalVarString
                            (*(undefined8 *)(param_1 + 8),lVar7 + 0x29,pvVar13,iVar5,uVar3);
          if ((iVar5 != 0) && (cVar4 = HSlog_ShouldLog(4), cVar4 != '\0')) {
            p_Var10 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
            uVar11 = FUN_180003ec0(p_Var10);
            HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] VM_M_BatchSetGlobalVarString is error:[%x]",
                           uVar11,*(undefined4 *)(param_1 + 0x48),
                           "CVariableModuBase::SetGlobalValue",0x633,iVar5);
          }
          operator_delete__(pvVar13);
        }
      }
    }
    VM_M_SetGlobalVarLockStatus(*(undefined4 *)(param_1 + 0x48),*(undefined4 *)(param_1 + 0x48),0);
  }
  lVar7 = FUN_180009510(param_2);
  if (*(int *)(lVar7 + 0x450) == 0) {
    lVar7 = FUN_180009510(param_2);
    pbVar8 = (byte *)(lVar7 + 0x129);
    lVar7 = (longlong)&DAT_1800579dc - (longlong)pbVar8;
    do {
      bVar1 = *pbVar8;
      if (bVar1 != pbVar8[lVar7]) {
        uVar6 = -(uint)(bVar1 < pbVar8[lVar7]) | 1;
        goto LAB_1800275f7;
      }
      pbVar8 = pbVar8 + 1;
    } while (bVar1 != 0);
    uVar6 = 0;
LAB_1800275f7:
    if (uVar6 == 0) {
      lVar7 = FUN_180009510(param_2);
      iVar5 = atoi((char *)(lVar7 + 0x139));
      lVar7 = FUN_180009510(param_2);
      VM_M_SetGlobalVarInt(*(undefined8 *)(param_1 + 8),lVar7 + 0x29,0,iVar5);
    }
    else {
      lVar7 = FUN_180009510(param_2);
      pbVar8 = (byte *)(lVar7 + 0x129);
      lVar7 = (longlong)"float" - (longlong)pbVar8;
      do {
        bVar1 = *pbVar8;
        if (bVar1 != pbVar8[lVar7]) {
          uVar6 = -(uint)(bVar1 < pbVar8[lVar7]) | 1;
          goto LAB_180027685;
        }
        pbVar8 = pbVar8 + 1;
      } while (bVar1 != 0);
      uVar6 = 0;
LAB_180027685:
      if (uVar6 == 0) {
        lVar7 = FUN_180009510(param_2);
        dVar15 = atof((char *)(lVar7 + 0x139));
        lVar7 = FUN_180009510(param_2);
        VM_M_SetGlobalVarFloat(*(undefined8 *)(param_1 + 8),lVar7 + 0x29,0,(float)dVar15);
      }
      else {
        lVar7 = FUN_180009510(param_2);
        pbVar8 = (byte *)(lVar7 + 0x129);
        lVar7 = (longlong)"string" - (longlong)pbVar8;
        do {
          bVar1 = *pbVar8;
          if (bVar1 != pbVar8[lVar7]) {
            uVar6 = -(uint)(bVar1 < pbVar8[lVar7]) | 1;
            goto LAB_18002771b;
          }
          pbVar8 = pbVar8 + 1;
        } while (bVar1 != 0);
        uVar6 = 0;
LAB_18002771b:
        if (uVar6 == 0) {
          lVar7 = FUN_180009510(param_2);
          lVar9 = FUN_180009510(param_2);
          VM_M_SetGlobalVarString(*(undefined8 *)(param_1 + 8),lVar9 + 0x29,0,lVar7 + 0x139);
        }
      }
    }
  }
  local_90 = 0;
  while( true ) {
    lVar7 = FUN_180009510(param_2);
    if ((ulonglong)(longlong)*(int *)(lVar7 + 0x43c) <= local_90) {
      FUN_180016480((longlong)param_2);
      return;
    }
    lVar7 = FUN_180009510(param_2);
    local_80 = -1;
    do {
      local_80 = local_80 + 1;
    } while (*(char *)(*(longlong *)(lVar7 + 0x440) + 0x14 + local_90 * 0x94 + local_80) != '\0');
    if (local_80 == 0) break;
    lVar7 = FUN_180009510(param_2);
    local_88 = 0xffffffffffffffff;
    do {
      local_88 = local_88 + 1;
    } while (*(char *)(lVar7 + 0x139 + local_88) != '\0');
    lVar9 = FUN_180009510(param_2);
    lVar7 = FUN_180009510(param_2);
    lVar7 = *(longlong *)(lVar7 + 0x440);
    lVar14 = FUN_180009510(param_2);
    VM_M_SetBinayData(*(undefined4 *)(*(longlong *)(lVar14 + 0x440) + local_90 * 0x94),
                      lVar7 + 0x14 + local_90 * 0x94,lVar9 + 0x139,local_88 & 0xffffffff);
    local_90 = local_90 + 1;
  }
  FUN_180016480((longlong)param_2);
  return;
}

