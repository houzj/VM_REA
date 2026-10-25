
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180098360(undefined8 param_1,undefined4 param_2,undefined8 *param_3,int param_4,
                  char *param_5)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  char cVar4;
  int iVar5;
  undefined8 uVar6;
  undefined8 *puVar7;
  longlong lVar8;
  undefined1 auStack_f8 [32];
  undefined4 uStack_d8;
  undefined4 uStack_d0;
  undefined8 *puStack_c8;
  int iStack_c0;
  longlong lStack_b8;
  longlong *plStack_b0;
  longlong lStack_a8;
  longlong *plStack_a0;
  undefined1 auStack_98 [16];
  undefined8 uStack_88;
  undefined8 uStack_80;
  undefined8 uStack_78;
  undefined8 uStack_70;
  undefined1 uStack_68;
  undefined7 uStack_67;
  undefined8 uStack_58;
  ulonglong uStack_50;
  ulonglong uStack_48;
  
  uStack_70 = 0xfffffffffffffffe;
  uStack_48 = _DAT_1802a5230 ^ (ulonglong)auStack_f8;
  if (param_5 == (char *)0x0) {
    cVar4 = FUN_18001dbc0(4);
    if (cVar4 != '\0') {
      uVar6 = FUN_18001db50();
      uStack_d8 = 0x452;
      FUN_180015b40(uVar6,4,&UNK_1802246d0,&UNK_180226818);
    }
  }
  else {
    if (((param_4 == 0) && (iVar5 = FUN_18009a7e0(), iVar5 != 0)) &&
       (cVar4 = FUN_18001dbc0(4), cVar4 != '\0')) {
      puVar7 = param_3;
      if (0xf < (ulonglong)param_3[3]) {
        puVar7 = (undefined8 *)*param_3;
      }
      uVar6 = FUN_18001db50();
      uStack_d8 = 0x45c;
      uStack_d0 = param_2;
      puStack_c8 = puVar7;
      iStack_c0 = iVar5;
      FUN_180015b40(uVar6,4,&UNK_1802268b0,&UNK_180226818);
    }
    uStack_78 = 0x1802b1208;
    FUN_1801e5430(0x1802b1208);
    uStack_50 = 0xf;
    lVar8 = 0;
    uStack_58 = 0;
    uStack_68 = 0;
    if (*param_5 != '\0') {
      lVar8 = -1;
      do {
        lVar8 = lVar8 + 1;
      } while (param_5[lVar8] != '\0');
    }
    FUN_180006300(&uStack_68,param_5,lVar8);
    if ((_DAT_1802b2b48 & 1) == 0) {
      _DAT_1802b2b48 = _DAT_1802b2b48 | 1;
      FUN_1800c8f10(0x1802b2b50);
      FUN_1801e8930(&UNK_180202a80);
    }
    FUN_1800cae00(0x1802b2b50,&lStack_b8,param_2);
    plVar3 = plStack_b0;
    if (lStack_b8 == 0) {
      if (plStack_b0 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_b0 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_b0)(plStack_b0);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_b0 + 8))();
          }
        }
      }
    }
    else {
      FUN_1800f7260(lStack_b8,&lStack_a8,param_3);
      if (lStack_a8 == 0) {
        if (plStack_a0 != (longlong *)0x0) {
          LOCK();
          plVar3 = plStack_a0 + 1;
          *(int *)plVar3 = (int)*plVar3 + -1;
          UNLOCK();
          if ((int)*plVar3 == 0) {
            (**(code **)*plStack_a0)(plStack_a0);
            LOCK();
            piVar2 = (int *)((longlong)plStack_a0 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plStack_a0 + 8))(plStack_a0);
            }
          }
        }
      }
      else {
        uStack_80 = 0xf;
        uStack_88 = 0;
        auStack_98[0] = 0;
        FUN_180006470(auStack_98,&uStack_68,0,0xffffffffffffffff);
        FUN_18000ffe0(lStack_a8,param_4,auStack_98);
        if (plStack_a0 != (longlong *)0x0) {
          LOCK();
          plVar3 = plStack_a0 + 1;
          *(int *)plVar3 = (int)*plVar3 + -1;
          UNLOCK();
          if ((int)*plVar3 == 0) {
            (**(code **)*plStack_a0)(plStack_a0);
            LOCK();
            piVar2 = (int *)((longlong)plStack_a0 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plStack_a0 + 8))(plStack_a0);
            }
          }
        }
      }
      plVar3 = plStack_b0;
      if (plStack_b0 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_b0 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_b0)(plStack_b0);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_b0 + 8))();
          }
        }
      }
    }
    if (0xf < uStack_50) {
      __3_YAXPEAX_Z(CONCAT71(uStack_67,uStack_68));
    }
    FUN_1801e5390(0x1802b1208);
  }
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_f8);
  return;
}

