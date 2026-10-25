
undefined4
FUN_180099550(longlong param_1,undefined4 param_2,undefined8 *param_3,longlong param_4,int param_5,
             char param_6)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  char cVar4;
  int iVar5;
  undefined4 uVar6;
  longlong lVar7;
  undefined8 uVar8;
  undefined8 *puVar9;
  longlong lVar10;
  undefined4 auStackX_10 [2];
  undefined8 uVar11;
  longlong lStack_38;
  longlong *plStack_30;
  
  uVar11 = 0xfffffffffffffffe;
  if (param_5 == 0) {
    uVar6 = 0xe0000001;
  }
  else {
    auStackX_10[0] = param_2;
    if (param_6 != '\0') {
      lVar7 = FUN_18009a990(param_1 + 0x18,auStackX_10);
      lVar7 = FUN_180007510(*(undefined8 *)(lVar7 + 0x18));
      FUN_18003fc70(lVar7,&DAT_180225f90);
      *(uint *)(lVar7 + 0x10) = *(uint *)(lVar7 + 0x10) | 1;
      puVar9 = *(undefined8 **)(lVar7 + 0x18);
      if (puVar9 == DAT_1802b05e8) {
        FUN_180001ac0(lVar7 + 0x18,param_3);
      }
      else if (puVar9 != param_3) {
        FUN_180006470(puVar9,param_3,0,0xffffffffffffffff);
      }
      lVar10 = 0;
      if (0 < (longlong)param_5) {
        do {
          uVar6 = *(undefined4 *)(param_4 + lVar10 * 4);
          if (*(int *)(lVar7 + 0x40) == *(int *)(lVar7 + 0x44)) {
            FUN_18000ee00(lVar7 + 0x40,*(int *)(lVar7 + 0x44) + 1);
          }
          *(undefined4 *)(*(longlong *)(lVar7 + 0x48) + 8 + (longlong)*(int *)(lVar7 + 0x40) * 4) =
               uVar6;
          *(int *)(lVar7 + 0x40) = *(int *)(lVar7 + 0x40) + 1;
          lVar10 = lVar10 + 1;
        } while (lVar10 < param_5);
      }
    }
    iVar5 = FUN_18009a7e0();
    if ((iVar5 != 0) && (cVar4 = FUN_18001dbc0(4), cVar4 != '\0')) {
      puVar9 = param_3;
      if (0xf < (ulonglong)param_3[3]) {
        puVar9 = (undefined8 *)*param_3;
      }
      uVar8 = FUN_18001db50();
      FUN_180015b40(uVar8,4,&DAT_180226930,&DAT_180226b58,0x527,auStackX_10[0],puVar9,iVar5,uVar11);
    }
    FUN_1801e5430(&DAT_1802b1208);
    if ((DAT_1802b2b48 & 1) == 0) {
      DAT_1802b2b48 = DAT_1802b2b48 | 1;
      FUN_1800c8f10(&DAT_1802b2b50);
      FUN_1801e8930(&DAT_180202a80);
    }
    FUN_1800cae00(&DAT_1802b2b50,&lStack_38,auStackX_10[0]);
    plVar3 = plStack_30;
    if (lStack_38 == 0) {
      if (plStack_30 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_30 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_30)(plStack_30);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_30 + 8))();
          }
        }
      }
      uVar6 = 0xe0000003;
    }
    else {
      uVar6 = FUN_1800f4dc0(lStack_38,param_3,param_4,param_5);
      plVar3 = plStack_30;
      if (plStack_30 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_30 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_30)(plStack_30);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_30 + 8))();
          }
        }
      }
    }
    FUN_1801e5390(&DAT_1802b1208);
  }
  return uVar6;
}

