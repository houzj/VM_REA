
void FUN_1800f66f0(longlong param_1,longlong *param_2,int param_3,undefined8 *param_4,
                  longlong *param_5,longlong *param_6,longlong param_7,longlong *param_8)

{
  int *piVar1;
  int iVar2;
  longlong lVar3;
  char cVar4;
  int iVar5;
  longlong lVar6;
  longlong *plVar7;
  undefined8 uVar8;
  undefined1 *puVar9;
  undefined1 auStack_f8 [32];
  longlong lStack_d8;
  longlong *plStack_d0;
  longlong *plStack_c8;
  longlong *plStack_c0;
  longlong *plStack_b8;
  longlong lStack_b0;
  undefined8 *puStack_a8;
  longlong lStack_a0;
  longlong lStack_98;
  undefined4 uStack_90;
  longlong lStack_88;
  longlong *plStack_80;
  undefined8 uStack_78;
  undefined1 uStack_70;
  undefined7 uStack_6f;
  undefined8 uStack_60;
  ulonglong uStack_58;
  ulonglong uStack_50;
  
  uStack_78 = 0xfffffffffffffffe;
  uStack_50 = DAT_1802a5230 ^ (ulonglong)auStack_f8;
  plStack_b8 = param_6;
  lStack_b0 = param_7;
  uStack_90 = 0;
  lVar3 = *(longlong *)(param_1 + 0x40);
  puStack_a8 = param_4;
  lVar6 = FUN_1800202d0(param_1 + 0x40);
  if ((lVar6 == *(longlong *)(param_1 + 0x40)) ||
     (iVar5 = FUN_180020a60(param_2,lVar6 + 0x20), iVar5 < 0)) {
    lStack_98 = *(longlong *)(param_1 + 0x40);
    plVar7 = &lStack_98;
  }
  else {
    lStack_a0 = lVar6;
    plVar7 = &lStack_a0;
  }
  if (*plVar7 == lVar3) goto LAB_1800f6a9f;
  FUN_1800f79b0(param_1,&lStack_88,param_2);
  if (lStack_88 == 0) {
    cVar4 = FUN_18001dbc0(4);
    if (cVar4 != '\0') {
      if (0xf < (ulonglong)param_2[3]) {
        param_2 = (longlong *)*param_2;
      }
      uVar8 = FUN_18001db50();
      lStack_d8 = CONCAT44(lStack_d8._4_4_,0x366);
      plStack_d0 = param_2;
      FUN_180015b40(uVar8,4,&DAT_18022eba8,&DAT_18022eaf8);
    }
  }
  else {
    uStack_58 = 0xf;
    uStack_60 = 0;
    uStack_70 = 0;
    if (param_3 < 0) {
      iVar5 = -0x1fffffec;
LAB_1800f6851:
      cVar4 = FUN_18001dbc0(1);
      if (cVar4 != '\0') {
        if (0xf < (ulonglong)param_2[3]) {
          param_2 = (longlong *)*param_2;
        }
        uVar8 = FUN_18001db50();
        plStack_c0 = (longlong *)CONCAT44(plStack_c0._4_4_,iVar5);
        plStack_c8 = (longlong *)CONCAT44(plStack_c8._4_4_,param_3);
        lStack_d8 = CONCAT44(lStack_d8._4_4_,0x36e);
        plStack_d0 = param_2;
        FUN_180015b40(uVar8,1,&DAT_18022eb60,&DAT_18022eaf8);
      }
    }
    else {
      lVar3 = *(longlong *)(lStack_88 + 0x10);
      if (lVar3 == 0) {
        iVar5 = -0x1ffffcec;
        goto LAB_1800f6851;
      }
      iVar2 = *(int *)(lVar3 + 0x30);
      *(int *)plStack_b8 = iVar2;
      if (param_3 < iVar2) {
        puVar9 = *(undefined1 **)(*(longlong *)(lVar3 + 0x38) + 8 + (longlong)param_3 * 8);
        if (&uStack_70 != puVar9) {
          FUN_180006470(&uStack_70,puVar9,0,0xffffffffffffffff);
        }
      }
      else {
        iVar5 = -0x1fffffec;
        if (iVar2 == 0) {
          iVar5 = -0x1ffffcec;
        }
        if (iVar5 != 0) goto LAB_1800f6851;
      }
      plStack_b8 = (longlong *)0x0;
      if (lStack_b0 == 0) {
        puVar9 = &uStack_70;
        if (0xf < uStack_58) {
          puVar9 = (undefined1 *)CONCAT71(uStack_6f,uStack_70);
        }
        iVar5 = sscanf_s(puVar9,&DAT_180226ac4,param_5,&plStack_b8);
        if ((iVar5 != 2) && (cVar4 = FUN_18001dbc0(4), cVar4 != '\0')) {
          plVar7 = (longlong *)&uStack_70;
          if (0xf < uStack_58) {
            plVar7 = (longlong *)CONCAT71(uStack_6f,uStack_70);
          }
          uVar8 = FUN_18001db50();
          lStack_d8 = CONCAT44(lStack_d8._4_4_,899);
          plStack_d0 = plVar7;
          FUN_180015b40(uVar8,4,&DAT_18022ebd0,&DAT_18022eaf8);
        }
      }
      else {
        puVar9 = &uStack_70;
        if (0xf < uStack_58) {
          puVar9 = (undefined1 *)CONCAT71(uStack_6f,uStack_70);
        }
        plStack_d0 = param_8;
        lStack_d8 = lStack_b0;
        iVar5 = sscanf_s(puVar9,&DAT_180226ab8);
        if ((iVar5 != 3) && (cVar4 = FUN_18001dbc0(4), cVar4 != '\0')) {
          plVar7 = (longlong *)&uStack_70;
          if (0xf < uStack_58) {
            plVar7 = (longlong *)CONCAT71(uStack_6f,uStack_70);
          }
          uVar8 = FUN_18001db50();
          plStack_c8 = param_8;
          lStack_d8 = CONCAT44(lStack_d8._4_4_,0x37b);
          plStack_d0 = plVar7;
          FUN_180015b40(uVar8,4,&DAT_18022ebe8,&DAT_18022eaf8);
        }
      }
      if ((*param_5 == 0) || (plStack_b8 == (longlong *)0x0)) {
        cVar4 = FUN_18001dbc0(4);
        if (cVar4 != '\0') {
          puVar9 = &uStack_70;
          if (0xf < uStack_58) {
            puVar9 = (undefined1 *)CONCAT71(uStack_6f,uStack_70);
          }
          uVar8 = FUN_18001db50();
          plStack_c8 = plStack_b8;
          plStack_d0 = (longlong *)*param_5;
          lStack_d8 = CONCAT44(lStack_d8._4_4_,0x38a);
          plStack_c0 = (longlong *)puVar9;
          FUN_180015b40(uVar8,4,&DAT_18022ec30,&DAT_18022eaf8);
        }
      }
      else {
        *puStack_a8 = plStack_b8;
      }
    }
    if (0xf < uStack_58) {
      __3_YAXPEAX_Z(CONCAT71(uStack_6f,uStack_70));
    }
    uStack_58 = 0xf;
    uStack_60 = 0;
    uStack_70 = 0;
  }
  if (plStack_80 != (longlong *)0x0) {
    LOCK();
    plVar7 = plStack_80 + 1;
    *(int *)plVar7 = (int)*plVar7 + -1;
    UNLOCK();
    if ((int)*plVar7 == 0) {
      (**(code **)*plStack_80)(plStack_80);
      LOCK();
      piVar1 = (int *)((longlong)plStack_80 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_80 + 8))(plStack_80);
      }
    }
  }
LAB_1800f6a9f:
  FUN_1801e8810(uStack_50 ^ (ulonglong)auStack_f8);
  return;
}

