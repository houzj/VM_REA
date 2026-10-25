
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800144b0(longlong param_1,longlong param_2,int *param_3,int param_4,int *param_5)

{
  longlong *plVar1;
  undefined8 *puVar2;
  char cVar3;
  int iVar4;
  int iVar5;
  undefined8 *puVar6;
  longlong *plVar7;
  undefined8 uVar8;
  char *pcVar9;
  ulonglong uVar10;
  undefined *puVar11;
  int *piVar12;
  int *piVar13;
  undefined1 auStack_108 [32];
  int *piStack_e8;
  undefined4 uStack_e0;
  longlong lStack_d8;
  int *piStack_d0;
  int iStack_c8;
  int *piStack_b8;
  int *piStack_b0;
  undefined8 uStack_a8;
  longlong lStack_a0;
  undefined8 auStack_98 [2];
  undefined8 uStack_88;
  ulonglong uStack_80;
  undefined1 uStack_78;
  undefined7 uStack_77;
  undefined8 uStack_68;
  ulonglong uStack_60;
  ulonglong uStack_58;
  
  uStack_a8 = 0xfffffffffffffffe;
  uStack_58 = _DAT_180025000 ^ (ulonglong)auStack_108;
  piStack_b8 = param_5;
  uVar10 = 0;
  iVar5 = 0;
  piStack_b0 = param_3;
  if ((((param_2 == 0) || (param_3 == (int *)0x0)) || (param_4 < 1)) || (param_5 == (int *)0x0)) {
    cVar3 = HSlog_ShouldLog(4);
    if (cVar3 != '\0') {
      if ((_DAT_180025eb0 & 1) == 0) {
        _DAT_180025eb0 = _DAT_180025eb0 | 1;
        FUN_1800013f0();
        FUN_180018df8(FUN_18001a860);
      }
      puVar11 = &DAT_180025e90;
      if (0xf < _DAT_180025ea8) {
        puVar11 = _DAT_180025e90;
      }
      uStack_e0 = 0x1f5;
      piStack_e8 = (int *)&UNK_18001d128;
      lStack_d8 = param_2;
      piStack_d0 = param_3;
      iStack_c8 = param_4;
      HSlog_LogPrint(4,&UNK_18001d150,puVar11,*(undefined4 *)(param_1 + 0x48));
    }
    goto LAB_180014de3;
  }
  cVar3 = HSlog_ShouldLog(2);
  if (cVar3 != '\0') {
    puVar6 = (undefined8 *)FUN_1800013b0();
    if (0xf < (ulonglong)puVar6[3]) {
      puVar6 = (undefined8 *)*puVar6;
    }
    uStack_e0 = 0x1f9;
    piStack_e8 = (int *)&UNK_18001d128;
    lStack_d8 = param_2;
    HSlog_LogPrint(2,&UNK_18001d198,puVar6,*(undefined4 *)(param_1 + 0x48));
  }
  iVar4 = func_0x00018001986e(param_2,&UNK_18001d1c0);
  if (iVar4 == 0) {
    iVar4 = _Mtx_lock(param_1 + 0x70);
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
    plVar1 = *(longlong **)(param_1 + 0x60);
    plVar7 = (longlong *)*plVar1;
    if (plVar7 != plVar1) {
      iVar4 = 1;
      piVar12 = param_3;
      do {
        piVar13 = piVar12;
        if (((int *)plVar7[2])[1] == 1) {
          uVar10 = (ulonglong)((int)uVar10 + 1);
          iVar4 = iVar4 + 2;
          piVar13 = piVar12 + 2;
          if ((ulonglong)((longlong)iVar4 << 2) <= (ulonglong)(longlong)param_4) {
            piVar12[1] = *(int *)plVar7[2];
            *piVar13 = *(int *)(plVar7[2] + 0x18);
          }
        }
        iVar5 = (int)uVar10;
        plVar7 = (longlong *)*plVar7;
        piVar12 = piVar13;
        param_5 = piStack_b8;
      } while (plVar7 != plVar1);
    }
    if (4 < (ulonglong)(longlong)param_4) {
      *param_3 = iVar5;
    }
    *param_5 = iVar5 * 8 + 4;
  }
  else {
    iVar5 = func_0x00018001986e(param_2,&UNK_18001d1d0);
    if (iVar5 != 0) {
      iVar5 = func_0x00018001986e(param_2,&UNK_18001d1e0);
      if (iVar5 == 0) {
        if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180014de3;
        FUN_1800129d0(&uStack_78,*(undefined4 *)(*(longlong *)(param_1 + 0x50) + 4));
        uStack_80 = 0xf;
        uStack_88 = 0;
        auStack_98[0]._0_1_ = 0;
        FUN_180002a10(auStack_98,&uStack_78,0,0xffffffffffffffff);
        piStack_e8 = (int *)auStack_98;
        FUN_180015ad0();
      }
      else {
        iVar5 = func_0x00018001986e(param_2,&UNK_18001d1f0);
        if (iVar5 == 0) {
          if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180014de3;
          FUN_1800129d0(&uStack_78,*(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x18));
          uStack_80 = 0xf;
          uStack_88 = 0;
          auStack_98[0]._0_1_ = 0;
          FUN_180002a10(auStack_98,&uStack_78,0,0xffffffffffffffff);
          piStack_e8 = (int *)auStack_98;
          FUN_180015ad0();
        }
        else {
          iVar5 = func_0x00018001986e(param_2,&UNK_18001d200);
          if (iVar5 == 0) {
            if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180014de3;
            uStack_60 = 0xf;
            uStack_68 = 0;
            uStack_78 = 0;
            uVar8 = FUN_1800129d0(auStack_98,*(undefined4 *)(*(longlong *)(param_1 + 0x50) + 8));
            uVar8 = FUN_1800027a0(&uStack_78,uVar8,0,0xffffffffffffffff);
            FUN_1800028c0(uVar8,&UNK_18001d210,1);
            if (0xf < uStack_80) {
              __3_YAXPEAX_Z(CONCAT71(auStack_98[0]._1_7_,(undefined1)auStack_98[0]));
            }
            uVar8 = FUN_1800129d0(auStack_98,*(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x10));
            uVar8 = FUN_1800027a0(&uStack_78,uVar8,0,0xffffffffffffffff);
            FUN_1800028c0(uVar8,&UNK_18001d210,1);
            if (0xf < uStack_80) {
              __3_YAXPEAX_Z(CONCAT71(auStack_98[0]._1_7_,(undefined1)auStack_98[0]));
            }
            uVar8 = FUN_1800169c0();
            uVar8 = FUN_1800027a0(&uStack_78,uVar8,0,0xffffffffffffffff);
            FUN_1800028c0(uVar8,&UNK_18001d210,1);
            if (0xf < uStack_80) {
              __3_YAXPEAX_Z(CONCAT71(auStack_98[0]._1_7_,(undefined1)auStack_98[0]));
            }
            if (*(int *)(*(longlong *)(param_1 + 0x50) + 0xc) == 0x10) {
              uVar8 = FUN_1800129d0(auStack_98,*(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x28)
                                   );
              uVar8 = FUN_1800027a0(&uStack_78,uVar8,0,0xffffffffffffffff);
              FUN_1800028c0(uVar8,&UNK_18001d210);
              if (0xf < uStack_80) {
                __3_YAXPEAX_Z(CONCAT71(auStack_98[0]._1_7_,(undefined1)auStack_98[0]));
              }
              pcVar9 = (char *)(*(longlong *)(param_1 + 0x50) + 0x2c);
              if (*pcVar9 != '\0') {
                uVar10 = 0xffffffffffffffff;
                do {
                  uVar10 = uVar10 + 1;
                } while (pcVar9[uVar10] != '\0');
              }
              FUN_1800028c0(&uStack_78,pcVar9,uVar10);
            }
            uStack_80 = 0xf;
            uStack_88 = 0;
            auStack_98[0]._0_1_ = 0;
            FUN_180002a10(auStack_98,&uStack_78,0,0xffffffffffffffff);
            piStack_e8 = (int *)auStack_98;
            FUN_180015ad0();
          }
          else {
            iVar5 = func_0x00018001986e(param_2,&UNK_18001d218);
            if (iVar5 == 0) {
              if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180014de3;
              uStack_60 = 0xf;
              uStack_68 = 0;
              uStack_78 = 0;
              uVar8 = FUN_1800129d0(auStack_98,*(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x10)
                                   );
              uVar8 = FUN_1800027a0(&uStack_78,uVar8,0,0xffffffffffffffff);
              FUN_1800028c0(uVar8,&UNK_18001d210,1);
              if (0xf < uStack_80) {
                __3_YAXPEAX_Z(CONCAT71(auStack_98[0]._1_7_,(undefined1)auStack_98[0]));
              }
              uVar8 = FUN_1800129d0(auStack_98,*(undefined4 *)(*(longlong *)(param_1 + 0x50) + 8));
              uVar8 = FUN_1800027a0(&uStack_78,uVar8,0,0xffffffffffffffff);
              FUN_1800028c0(uVar8,&UNK_18001d210,1);
              if (0xf < uStack_80) {
                __3_YAXPEAX_Z(CONCAT71(auStack_98[0]._1_7_,(undefined1)auStack_98[0]));
              }
              uVar8 = FUN_1800129d0(auStack_98,*(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x28)
                                   );
              uVar8 = FUN_1800027a0(&uStack_78,uVar8,0,0xffffffffffffffff);
              FUN_1800028c0(uVar8,&UNK_18001d210);
              if (0xf < uStack_80) {
                __3_YAXPEAX_Z(CONCAT71(auStack_98[0]._1_7_,(undefined1)auStack_98[0]));
              }
              pcVar9 = (char *)(*(longlong *)(param_1 + 0x50) + 0x2c);
              if (*pcVar9 != '\0') {
                uVar10 = 0xffffffffffffffff;
                do {
                  uVar10 = uVar10 + 1;
                } while (pcVar9[uVar10] != '\0');
              }
              FUN_1800028c0(&uStack_78,pcVar9,uVar10);
              uStack_80 = 0xf;
              uStack_88 = 0;
              auStack_98[0]._0_1_ = 0;
              FUN_180002a10(auStack_98,&uStack_78,0,0xffffffffffffffff);
              piStack_e8 = (int *)auStack_98;
              FUN_180015ad0();
            }
            else {
              iVar5 = func_0x00018001986e(param_2,&UNK_18001d228);
              if (iVar5 == 0) {
                if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180014de3;
                FUN_1800129d0(&uStack_78,*(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x14));
                uStack_80 = 0xf;
                uStack_88 = 0;
                auStack_98[0]._0_1_ = 0;
                FUN_180002a10(auStack_98,&uStack_78,0,0xffffffffffffffff);
                piStack_e8 = (int *)auStack_98;
                FUN_180015ad0();
              }
              else {
                iVar5 = func_0x00018001986e(param_2,&UNK_18001d238);
                if (iVar5 == 0) {
                  if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180014de3;
                  FUN_1800129d0(&uStack_78,*(int *)(*(longlong *)(param_1 + 0x50) + 0x1c) + 1);
                  uStack_80 = 0xf;
                  uStack_88 = 0;
                  auStack_98[0]._0_1_ = 0;
                  FUN_180002a10(auStack_98,&uStack_78,0,0xffffffffffffffff);
                  piStack_e8 = (int *)auStack_98;
                  FUN_180015ad0();
                }
                else {
                  iVar5 = func_0x00018001986e(param_2,&UNK_18001d248);
                  if (iVar5 == 0) {
                    if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180014de3;
                    FUN_1800129d0(&uStack_78,*(int *)(*(longlong *)(param_1 + 0x50) + 0x20) + 1);
                    uStack_80 = 0xf;
                    uStack_88 = 0;
                    auStack_98[0]._0_1_ = 0;
                    FUN_180002a10(auStack_98,&uStack_78,0,0xffffffffffffffff);
                    piStack_e8 = (int *)auStack_98;
                    FUN_180015ad0();
                  }
                  else {
                    iVar5 = func_0x00018001986e(param_2,&UNK_18001d258);
                    if (iVar5 != 0) {
                      piStack_e8 = param_5;
                      FUN_180008460(param_1,param_2,param_3,param_4);
                      goto LAB_180014de3;
                    }
                    if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180014de3;
                    FUN_1800129d0(&uStack_78,*(int *)(*(longlong *)(param_1 + 0x50) + 0x24) + 1);
                    uStack_80 = 0xf;
                    uStack_88 = 0;
                    auStack_98[0]._0_1_ = 0;
                    FUN_180002a10(auStack_98,&uStack_78,0,0xffffffffffffffff);
                    piStack_e8 = (int *)auStack_98;
                    FUN_180015ad0();
                  }
                }
              }
            }
          }
        }
      }
      if (0xf < uStack_60) {
        __3_YAXPEAX_Z(CONCAT71(uStack_77,uStack_78));
      }
      goto LAB_180014de3;
    }
    lStack_a0 = param_1 + 0x70;
    iVar5 = _Mtx_lock(lStack_a0);
    if (iVar5 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar5);
    }
    uStack_60 = 0xf;
    uStack_68 = 0;
    uStack_78 = 0;
    puVar6 = *(undefined8 **)(param_1 + 0x60);
    for (puVar2 = (undefined8 *)*puVar6; puVar2 != puVar6; puVar2 = (undefined8 *)*puVar2) {
      uVar8 = FUN_1800129d0(auStack_98,*(undefined4 *)puVar2[2]);
      uVar8 = FUN_1800027a0(&uStack_78,uVar8,0,0xffffffffffffffff);
      FUN_1800028c0(uVar8,&UNK_18001d1dc);
      if (0xf < uStack_80) {
        __3_YAXPEAX_Z(CONCAT71(auStack_98[0]._1_7_,(undefined1)auStack_98[0]));
      }
    }
    uStack_80 = 0xf;
    uStack_88 = 0;
    auStack_98[0]._0_1_ = 0;
    FUN_180002a10(auStack_98,&uStack_78,0,0xffffffffffffffff);
    piStack_e8 = (int *)auStack_98;
    FUN_180015ad0();
    if (0xf < uStack_60) {
      __3_YAXPEAX_Z(CONCAT71(uStack_77,uStack_78));
    }
    uStack_60 = 0xf;
    uStack_68 = 0;
    uStack_78 = 0;
  }
  iVar5 = _Mtx_unlock(param_1 + 0x70);
  if (iVar5 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar5);
  }
LAB_180014de3:
  FUN_180018cd0(uStack_58 ^ (ulonglong)auStack_108);
  return;
}

