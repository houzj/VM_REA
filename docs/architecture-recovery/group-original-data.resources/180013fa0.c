
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180013fa0(longlong param_1,longlong param_2)

{
  longlong lVar1;
  undefined1 *puVar2;
  char cVar3;
  int iVar4;
  undefined8 uVar5;
  longlong *plVar6;
  longlong *plVar7;
  longlong *plVar8;
  int iVar9;
  undefined1 auStack_168 [32];
  undefined4 uStack_148;
  longlong *plStack_140;
  undefined8 uStack_138;
  longlong *plStack_130;
  longlong *plStack_128;
  undefined8 *puStack_120;
  undefined4 uStack_118;
  undefined1 auStack_110 [16];
  undefined8 uStack_100;
  undefined8 uStack_f8;
  undefined1 auStack_f0 [16];
  undefined8 uStack_e0;
  undefined8 uStack_d8;
  undefined8 uStack_d0;
  undefined1 uStack_c8;
  undefined7 uStack_c7;
  undefined8 uStack_b8;
  ulonglong uStack_b0;
  undefined1 uStack_a8;
  undefined7 uStack_a7;
  undefined8 uStack_98;
  ulonglong uStack_90;
  undefined1 uStack_88;
  undefined7 uStack_87;
  undefined8 uStack_78;
  ulonglong uStack_70;
  undefined1 uStack_68;
  undefined7 uStack_67;
  undefined8 uStack_58;
  ulonglong uStack_50;
  ulonglong uStack_48;
  
  uStack_d0 = 0xfffffffffffffffe;
  uStack_48 = DAT_1802a5230 ^ (ulonglong)auStack_168;
  uStack_118 = 0;
  if (*(longlong *)(param_2 + 0x48) == 0) {
    cVar3 = FUN_18001dbc0(4);
    if (cVar3 != '\0') {
      uVar5 = FUN_18001db50();
      uStack_148 = 0x256;
      FUN_180015b40(uVar5,4,&DAT_18021c920,&DAT_18021c8f8);
    }
    goto LAB_1800145bd;
  }
  iVar4 = *(int *)(param_2 + 0x98);
  if (0 < iVar4) {
    if (iVar4 < 3) {
      uStack_d8 = 0xf;
      uStack_e0 = 0;
      auStack_f0[0] = 0;
      FUN_180006470(auStack_f0,param_2 + 0xb0,0,0xffffffffffffffff);
      iVar4 = FUN_1800100f0(*(undefined8 *)(param_2 + 0x48),auStack_f0);
      if ((iVar4 != 0) && (cVar3 = FUN_18001dbc0(4), cVar3 != '\0')) {
        plVar8 = (longlong *)FUN_180006450(param_2 + 8);
        uVar5 = FUN_18001db50();
        uStack_148 = 0x266;
        plStack_140 = plVar8;
        FUN_180015b40(uVar5,4,&DAT_18021c6f8,&DAT_18021c8f8);
      }
      goto LAB_1800145bd;
    }
    if (iVar4 == 3) {
      if (*(undefined8 **)(param_2 + 0x110) == (undefined8 *)0x0) {
        cVar3 = FUN_18001dbc0(4);
        if (cVar3 != '\0') {
          plVar8 = (longlong *)(param_1 + 0x10);
          if (0xf < *(ulonglong *)(param_1 + 0x28)) {
            plVar8 = (longlong *)*plVar8;
          }
          plVar6 = (longlong *)FUN_180006450(param_2 + 8);
          uVar5 = FUN_18001db50();
          uStack_138 = CONCAT44(uStack_138._4_4_,*(undefined4 *)(param_1 + 8));
          uStack_148 = 0x270;
          plStack_140 = plVar8;
          plStack_130 = plVar6;
          FUN_180015b40(uVar5,4,&DAT_18021c8b0,&DAT_18021c8f8);
        }
        goto LAB_1800145bd;
      }
      if (*(int *)(param_2 + 0x170) == 1) {
        cVar3 = (**(code **)**(undefined8 **)(param_2 + 0x110))();
        if (cVar3 == '\0') {
          if (*(longlong *)(*(longlong *)(param_2 + 0x110) + 0x10) != 0) {
            FUN_1800101a0(*(undefined8 *)(param_2 + 0x48));
          }
        }
        else {
          cVar3 = FUN_18001dbc0(1);
          if (cVar3 != '\0') {
            plVar8 = (longlong *)(param_2 + 0xd0);
            if (0xf < *(ulonglong *)(param_2 + 0xe8)) {
              plVar8 = (longlong *)*plVar8;
            }
            plVar6 = (longlong *)(param_1 + 0x10);
            if (0xf < *(ulonglong *)(param_1 + 0x28)) {
              plVar6 = (longlong *)*plVar6;
            }
            plVar7 = (longlong *)FUN_180006450(param_2 + 8);
            uVar5 = FUN_18001db50();
            uStack_138 = CONCAT44(uStack_138._4_4_,*(undefined4 *)(param_1 + 8));
            uStack_148 = 0x27a;
            plStack_140 = plVar6;
            plStack_130 = plVar7;
            plStack_128 = plVar8;
            FUN_180015b40(uVar5,1,&DAT_18021c760,&DAT_18021c8f8);
          }
        }
        goto LAB_1800145bd;
      }
      iVar4 = FUN_1800132e0();
      uStack_b0 = 0xf;
      uStack_b8 = 0;
      uStack_c8 = 0;
      if (iVar4 < 0) {
        iVar9 = -0x1fffffec;
LAB_1800141ef:
        cVar3 = FUN_18001dbc0(4);
        if (cVar3 != '\0') {
          puStack_120 = (undefined8 *)FUN_18008f750(&uStack_a8,iVar9);
          if (0xf < (ulonglong)puStack_120[3]) {
            puStack_120 = (undefined8 *)*puStack_120;
          }
          plVar8 = (longlong *)(param_2 + 0xd0);
          if (0xf < *(ulonglong *)(param_2 + 0xe8)) {
            plVar8 = (longlong *)*plVar8;
          }
          plVar6 = (longlong *)(param_1 + 0x10);
          if (0xf < *(ulonglong *)(param_1 + 0x28)) {
            plVar6 = (longlong *)*plVar6;
          }
          uVar5 = FUN_18001db50();
          plStack_128 = (longlong *)CONCAT44(plStack_128._4_4_,iVar4);
          uStack_138 = CONCAT44(uStack_138._4_4_,*(undefined4 *)(param_1 + 8));
          uStack_148 = 0x294;
          plStack_140 = plVar6;
          plStack_130 = plVar8;
          FUN_180015b40(uVar5,4,&DAT_18021c790,&DAT_18021c8f8);
          if (0xf < uStack_90) {
            __3_YAXPEAX_Z(CONCAT71(uStack_a7,uStack_a8));
          }
          uStack_90 = 0xf;
          uStack_98 = 0;
          uStack_a8 = 0;
        }
      }
      else {
        lVar1 = *(longlong *)(*(longlong *)(param_2 + 0x110) + 0x10);
        if (lVar1 == 0) {
          iVar9 = -0x1ffffcec;
LAB_1800142a7:
          cVar3 = FUN_18001dbc0(1);
          if (cVar3 != '\0') {
            puStack_120 = (undefined8 *)FUN_18008f750(&uStack_88,iVar9);
            if (0xf < (ulonglong)puStack_120[3]) {
              puStack_120 = (undefined8 *)*puStack_120;
            }
            plVar8 = (longlong *)(param_2 + 0xd0);
            if (0xf < *(ulonglong *)(param_2 + 0xe8)) {
              plVar8 = (longlong *)*plVar8;
            }
            plVar6 = (longlong *)(param_1 + 0x10);
            if (0xf < *(ulonglong *)(param_1 + 0x28)) {
              plVar6 = (longlong *)*plVar6;
            }
            uVar5 = FUN_18001db50();
            plStack_128 = (longlong *)CONCAT44(plStack_128._4_4_,iVar4);
            uStack_138 = CONCAT44(uStack_138._4_4_,*(undefined4 *)(param_1 + 8));
            uStack_148 = 0x290;
            plStack_140 = plVar6;
            plStack_130 = plVar8;
            FUN_180015b40(uVar5,1,&DAT_18021c790,&DAT_18021c8f8);
            if (0xf < uStack_70) {
              __3_YAXPEAX_Z(CONCAT71(uStack_87,uStack_88));
            }
            uStack_70 = 0xf;
            uStack_78 = 0;
            uStack_88 = 0;
          }
        }
        else {
          if (iVar4 < *(int *)(lVar1 + 0x30)) {
            puVar2 = *(undefined1 **)(*(longlong *)(lVar1 + 0x38) + 8 + (longlong)iVar4 * 8);
            if (&uStack_c8 != puVar2) {
              FUN_180006470(&uStack_c8,puVar2,0,0xffffffffffffffff);
            }
          }
          else {
            iVar9 = -0x1fffffec;
            if (*(int *)(lVar1 + 0x30) == 0) {
              iVar9 = -0x1ffffcec;
            }
            if (iVar9 != 0) {
              if ((iVar9 != -0x1ffffcec) && (iVar9 != -0x1ffffce5)) goto LAB_1800141ef;
              goto LAB_1800142a7;
            }
          }
          uStack_f8 = 0xf;
          uStack_100 = 0;
          auStack_110[0] = 0;
          FUN_180006470(auStack_110,&uStack_c8,0,0xffffffffffffffff);
          iVar4 = FUN_1800100f0(*(undefined8 *)(param_2 + 0x48),auStack_110);
          if ((iVar4 != 0) && (cVar3 = FUN_18001dbc0(4), cVar3 != '\0')) {
            plStack_128 = (longlong *)FUN_18008f750(&uStack_68,iVar4);
            if (0xf < (ulonglong)plStack_128[3]) {
              plStack_128 = (longlong *)*plStack_128;
            }
            plVar8 = (longlong *)(param_1 + 0x10);
            if (0xf < *(ulonglong *)(param_1 + 0x28)) {
              plVar8 = (longlong *)*plVar8;
            }
            plVar6 = (longlong *)FUN_180006450(param_2 + 8);
            uVar5 = FUN_18001db50();
            uStack_138 = CONCAT44(uStack_138._4_4_,*(undefined4 *)(param_1 + 8));
            uStack_148 = 0x29c;
            plStack_140 = plVar8;
            plStack_130 = plVar6;
            FUN_180015b40(uVar5,4,&DAT_18021c7d0,&DAT_18021c8f8);
            if (0xf < uStack_50) {
              __3_YAXPEAX_Z(CONCAT71(uStack_67,uStack_68));
            }
            uStack_50 = 0xf;
            uStack_58 = 0;
            uStack_68 = 0;
          }
        }
      }
      if (0xf < uStack_b0) {
        __3_YAXPEAX_Z(CONCAT71(uStack_c7,uStack_c8));
      }
      goto LAB_1800145bd;
    }
  }
  cVar3 = FUN_18001dbc0(4);
  if (cVar3 != '\0') {
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
    uStack_138 = FUN_180006450(param_2 + 8);
    plStack_140 = (longlong *)CONCAT44(plStack_140._4_4_,*(undefined4 *)(param_2 + 0x98));
    uStack_148 = 0x2a3;
    FUN_180015b40(&DAT_1802b1230,4,&DAT_18021c810,&DAT_18021c8f8);
  }
LAB_1800145bd:
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_168);
  return;
}

