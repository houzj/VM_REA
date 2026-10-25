
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180013450(longlong param_1,longlong param_2)

{
  longlong lVar1;
  char cVar2;
  int iVar3;
  undefined8 uVar4;
  longlong *plVar5;
  longlong *plVar6;
  longlong *plVar7;
  int iVar8;
  undefined4 uVar9;
  undefined1 auStack_f8 [32];
  undefined4 uStack_d8;
  longlong *plStack_d0;
  undefined8 uStack_c8;
  longlong *plStack_c0;
  longlong *plStack_b8;
  undefined8 *puStack_b0;
  undefined4 uStack_a8;
  undefined8 uStack_a0;
  undefined8 auStack_98 [3];
  ulonglong uStack_80;
  undefined8 auStack_78 [3];
  ulonglong uStack_60;
  undefined8 auStack_58 [3];
  ulonglong uStack_40;
  ulonglong uStack_38;
  
  uStack_a0 = 0xfffffffffffffffe;
  uStack_38 = DAT_1802a5230 ^ (ulonglong)auStack_f8;
  uVar9 = 0;
  uStack_a8 = 0;
  if (*(longlong *)(param_2 + 0x28) == 0) {
    cVar2 = FUN_18001dbc0(4);
    if (cVar2 != '\0') {
      uVar4 = FUN_18001db50();
      uStack_d8 = 0x19c;
      FUN_180015b40(uVar4,4,&DAT_18021c6c8,&DAT_18021c6a0);
    }
    goto LAB_1800139b6;
  }
  iVar3 = *(int *)(param_2 + 0x98);
  if (0 < iVar3) {
    if (iVar3 < 3) {
      iVar3 = FUN_18000fc50(*(longlong *)(param_2 + 0x28),*(undefined4 *)(param_2 + 0x9c));
      if ((iVar3 != 0) && (cVar2 = FUN_18001dbc0(4), cVar2 != '\0')) {
        plVar7 = (longlong *)FUN_180006450(param_2 + 8);
        uVar4 = FUN_18001db50();
        uStack_d8 = 0x1aa;
        plStack_d0 = plVar7;
        FUN_180015b40(uVar4,4,&DAT_18021c6f8,&DAT_18021c6a0);
      }
      goto LAB_1800139b6;
    }
    if (iVar3 == 3) {
      plVar7 = (longlong *)(param_2 + 0xd0);
      if (*(undefined8 **)(param_2 + 0xf0) == (undefined8 *)0x0) {
        cVar2 = FUN_18001dbc0(4);
        if (cVar2 != '\0') {
          plVar7 = (longlong *)(param_1 + 0x10);
          if (0xf < *(ulonglong *)(param_1 + 0x28)) {
            plVar7 = (longlong *)*plVar7;
          }
          plVar5 = (longlong *)FUN_180006450(param_2 + 8);
          uVar4 = FUN_18001db50();
          uStack_c8 = CONCAT44(uStack_c8._4_4_,*(undefined4 *)(param_1 + 8));
          uStack_d8 = 0x1b5;
          plStack_d0 = plVar7;
          plStack_c0 = plVar5;
          FUN_180015b40(uVar4,4,&DAT_18021c720,&DAT_18021c6a0);
        }
        goto LAB_1800139b6;
      }
      if (*(int *)(param_2 + 0x170) == 1) {
        cVar2 = (**(code **)**(undefined8 **)(param_2 + 0xf0))();
        if (cVar2 == '\0') {
          if (*(longlong *)(*(longlong *)(param_2 + 0xf0) + 0x10) != 0) {
            FUN_18000fcb0(*(undefined8 *)(param_2 + 0x28));
          }
        }
        else {
          cVar2 = FUN_18001dbc0(1);
          if (cVar2 != '\0') {
            if (0xf < *(ulonglong *)(param_2 + 0xe8)) {
              plVar7 = (longlong *)*plVar7;
            }
            plVar5 = (longlong *)(param_1 + 0x10);
            if (0xf < *(ulonglong *)(param_1 + 0x28)) {
              plVar5 = (longlong *)*plVar5;
            }
            plVar6 = (longlong *)FUN_180006450(param_2 + 8);
            uVar4 = FUN_18001db50();
            uStack_c8 = CONCAT44(uStack_c8._4_4_,*(undefined4 *)(param_1 + 8));
            uStack_d8 = 0x1bf;
            plStack_d0 = plVar5;
            plStack_c0 = plVar6;
            plStack_b8 = plVar7;
            FUN_180015b40(uVar4,1,&DAT_18021c760,&DAT_18021c6a0);
          }
        }
        goto LAB_1800139b6;
      }
      iVar3 = FUN_1800132e0();
      if (iVar3 < 0) {
        iVar8 = -0x1fffffec;
LAB_18001368e:
        cVar2 = FUN_18001dbc0(4);
        if (cVar2 != '\0') {
          puStack_b0 = (undefined8 *)FUN_18008f750(auStack_98,iVar8);
          if (0xf < (ulonglong)puStack_b0[3]) {
            puStack_b0 = (undefined8 *)*puStack_b0;
          }
          if (0xf < *(ulonglong *)(param_2 + 0xe8)) {
            plVar7 = (longlong *)*plVar7;
          }
          plVar5 = (longlong *)(param_1 + 0x10);
          if (0xf < *(ulonglong *)(param_1 + 0x28)) {
            plVar5 = (longlong *)*plVar5;
          }
          uVar4 = FUN_18001db50();
          plStack_b8 = (longlong *)CONCAT44(plStack_b8._4_4_,iVar3);
          uStack_c8 = CONCAT44(uStack_c8._4_4_,*(undefined4 *)(param_1 + 8));
          uStack_d8 = 0x1d9;
          plStack_d0 = plVar5;
          plStack_c0 = plVar7;
          FUN_180015b40(uVar4,4,&DAT_18021c790,&DAT_18021c6a0);
          if (0xf < uStack_80) {
            __3_YAXPEAX_Z(auStack_98[0]);
          }
        }
        goto LAB_1800139b6;
      }
      lVar1 = *(longlong *)(*(longlong *)(param_2 + 0xf0) + 0x10);
      if (lVar1 == 0) {
        iVar8 = -0x1ffffcec;
LAB_180013731:
        cVar2 = FUN_18001dbc0(1);
        if (cVar2 != '\0') {
          puStack_b0 = (undefined8 *)FUN_18008f750(auStack_78,iVar8);
          if (0xf < (ulonglong)puStack_b0[3]) {
            puStack_b0 = (undefined8 *)*puStack_b0;
          }
          if (0xf < *(ulonglong *)(param_2 + 0xe8)) {
            plVar7 = (longlong *)*plVar7;
          }
          plVar5 = (longlong *)(param_1 + 0x10);
          if (0xf < *(ulonglong *)(param_1 + 0x28)) {
            plVar5 = (longlong *)*plVar5;
          }
          uVar4 = FUN_18001db50();
          plStack_b8 = (longlong *)CONCAT44(plStack_b8._4_4_,iVar3);
          uStack_c8 = CONCAT44(uStack_c8._4_4_,*(undefined4 *)(param_1 + 8));
          uStack_d8 = 0x1d5;
          plStack_d0 = plVar5;
          plStack_c0 = plVar7;
          FUN_180015b40(uVar4,1,&DAT_18021c790,&DAT_18021c6a0);
          if (0xf < uStack_60) {
            __3_YAXPEAX_Z(auStack_78[0]);
          }
        }
      }
      else {
        if (iVar3 < *(int *)(lVar1 + 0x68)) {
          uVar9 = *(undefined4 *)(*(longlong *)(lVar1 + 0x70) + 8 + (longlong)iVar3 * 4);
        }
        else {
          iVar8 = -0x1fffffec;
          if (*(int *)(lVar1 + 0x68) == 0) {
            iVar8 = -0x1ffffcec;
          }
          if (iVar8 != 0) {
            if ((iVar8 != -0x1ffffcec) && (iVar8 != -0x1ffffce5)) goto LAB_18001368e;
            goto LAB_180013731;
          }
        }
        iVar3 = FUN_18000fc50(*(undefined8 *)(param_2 + 0x28),uVar9);
        if ((iVar3 != 0) && (cVar2 = FUN_18001dbc0(4), cVar2 != '\0')) {
          plStack_b8 = (longlong *)FUN_18008f750(auStack_58,iVar3);
          if (0xf < (ulonglong)plStack_b8[3]) {
            plStack_b8 = (longlong *)*plStack_b8;
          }
          plVar7 = (longlong *)(param_1 + 0x10);
          if (0xf < *(ulonglong *)(param_1 + 0x28)) {
            plVar7 = (longlong *)*plVar7;
          }
          plVar5 = (longlong *)FUN_180006450(param_2 + 8);
          uVar4 = FUN_18001db50();
          uStack_c8 = CONCAT44(uStack_c8._4_4_,*(undefined4 *)(param_1 + 8));
          uStack_d8 = 0x1e1;
          plStack_d0 = plVar7;
          plStack_c0 = plVar5;
          FUN_180015b40(uVar4,4,&DAT_18021c7d0,&DAT_18021c6a0);
          if (0xf < uStack_40) {
            __3_YAXPEAX_Z(auStack_58[0]);
          }
        }
      }
      goto LAB_1800139b6;
    }
  }
  cVar2 = FUN_18001dbc0(4);
  if (cVar2 != '\0') {
    if ((DAT_1802b1218 & 1) == 0) {
      DAT_1802b1218 = DAT_1802b1218 | 1;
      _DAT_1802b1230 = 1;
      DAT_1802b1238 = 0;
      DAT_1802b1240 = 0;
      uRam00000001802b1248 = 0;
      DAT_1802b1250 = 0;
      uRam00000001802b1258 = 0;
      DAT_1802b1260 = 0;
      uRam00000001802b1268 = 0;
      _DAT_1802b1270 = 0;
      FUN_1801e8930(&DAT_180202850);
    }
    uStack_c8 = FUN_180006450(param_2 + 8);
    plStack_d0 = (longlong *)CONCAT44(plStack_d0._4_4_,*(undefined4 *)(param_2 + 0x98));
    uStack_d8 = 0x1e8;
    FUN_180015b40(&DAT_1802b1230,4,&DAT_18021c810,&DAT_18021c6a0);
  }
LAB_1800139b6:
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_f8);
  return;
}

