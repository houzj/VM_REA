
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_BindGlobalVar(longlong param_1,int param_2)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  undefined8 *puVar4;
  longlong lVar5;
  char *pcVar6;
  longlong lVar7;
  int iVar8;
  longlong lVar9;
  undefined1 auStack_1c8 [32];
  undefined1 *puStack_1a8;
  undefined8 *puStack_1a0;
  undefined4 uStack_198;
  longlong lStack_190;
  undefined8 *puStack_188;
  undefined1 auStack_178 [272];
  undefined1 uStack_68;
  undefined7 uStack_67;
  undefined8 uStack_58;
  ulonglong uStack_50;
  ulonglong uStack_48;
  
                    /* 0x1417d0  47  VM_M_BindGlobalVar */
  uStack_48 = _DAT_1802a5230 ^ (ulonglong)auStack_1c8;
  if (DAT_1802b129e == '\0') {
    if (param_1 != 0) {
      cVar1 = FUN_18001dbc0(2);
      if (cVar1 != '\0') {
        uVar3 = FUN_18001db50();
        puStack_1a0 = (undefined8 *)CONCAT44(puStack_1a0._4_4_,param_2);
        puStack_1a8 = (undefined1 *)CONCAT44(puStack_1a8._4_4_,0x74b);
        FUN_180015b40(uVar3,2,&UNK_180238070,&UNK_180237fd8);
      }
      iVar2 = 0;
      iVar8 = 0;
      lVar9 = 0;
      if (0 < param_2) {
        pcVar6 = (char *)(param_1 + 8);
        do {
          lVar5 = (longlong)iVar8 * 0x358 + param_1;
          lVar7 = lVar5 + 0x88;
          FUN_180143f80(lVar7,auStack_178);
          cVar1 = FUN_18001dbc0(2);
          if (cVar1 != '\0') {
            puVar4 = (undefined8 *)FUN_180107380(&uStack_68,*(undefined4 *)(pcVar6 + -4));
            if (0xf < (ulonglong)puVar4[3]) {
              puVar4 = (undefined8 *)*puVar4;
            }
            uVar3 = FUN_18001db50();
            uStack_198 = *(undefined4 *)(pcVar6 + -8);
            puStack_1a0 = (undefined8 *)CONCAT44(puStack_1a0._4_4_,iVar8);
            puStack_1a8 = (undefined1 *)CONCAT44(puStack_1a8._4_4_,0x754);
            lStack_190 = lVar5 + 8;
            puStack_188 = puVar4;
            FUN_180015b40(uVar3,2,&UNK_1802380a0,&UNK_180237fd8);
            if (0xf < uStack_50) {
              __3_YAXPEAX_Z(CONCAT71(uStack_67,uStack_68));
            }
          }
          cVar1 = FUN_18001dbc0(2);
          if (cVar1 != '\0') {
            puVar4 = (undefined8 *)FUN_180107170(&uStack_68,lVar7);
            if (0xf < (ulonglong)puVar4[3]) {
              puVar4 = (undefined8 *)*puVar4;
            }
            uVar3 = FUN_18001db50();
            puStack_1a8 = (undefined1 *)CONCAT44(puStack_1a8._4_4_,0x755);
            puStack_1a0 = puVar4;
            FUN_180015b40(uVar3,2,&UNK_18021fa18,&UNK_180237fd8);
            if (0xf < uStack_50) {
              __3_YAXPEAX_Z(CONCAT71(uStack_67,uStack_68));
            }
          }
          uStack_50 = 0xf;
          uStack_58 = 0;
          uStack_68 = 0;
          if (*pcVar6 == '\0') {
            lVar7 = 0;
          }
          else {
            lVar7 = -1;
            do {
              lVar7 = lVar7 + 1;
            } while (pcVar6[lVar7] != '\0');
          }
          FUN_180006300(&uStack_68,pcVar6,lVar7);
          if ((_DAT_1802b2ec0 & 1) == 0) {
            _DAT_1802b2ec0 = _DAT_1802b2ec0 | 1;
            FUN_18009b340();
            FUN_1801e8930(&UNK_180202d50);
          }
          puStack_1a8 = auStack_178;
          iVar2 = FUN_18009b560(0x1802b2ed0,*(undefined4 *)(pcVar6 + -8),&uStack_68,
                                *(undefined4 *)(pcVar6 + -4));
          if (0xf < uStack_50) {
            __3_YAXPEAX_Z(CONCAT71(uStack_67,uStack_68));
          }
          if (iVar2 != 0) break;
          lVar9 = lVar9 + 1;
          iVar8 = iVar8 + 1;
          pcVar6 = pcVar6 + 0x358;
        } while (lVar9 < param_2);
      }
      cVar1 = FUN_18001dbc0(2);
      if (cVar1 != '\0') {
        puVar4 = (undefined8 *)FUN_18008f750(&uStack_68,iVar2);
        if (0xf < (ulonglong)puVar4[3]) {
          puVar4 = (undefined8 *)*puVar4;
        }
        uVar3 = FUN_18001db50();
        puStack_1a8 = (undefined1 *)CONCAT44(puStack_1a8._4_4_,0x75f);
        puStack_1a0 = puVar4;
        FUN_180015b40(uVar3,2,&UNK_180234258,&UNK_180237fd8);
        if (0xf < uStack_50) {
          __3_YAXPEAX_Z(CONCAT71(uStack_67,uStack_68));
        }
      }
    }
  }
  else {
    cVar1 = FUN_18001dbc0(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_18001db50();
      puStack_1a8 = (undefined1 *)CONCAT44(puStack_1a8._4_4_,0x744);
      FUN_180015b40(uVar3,4,&UNK_180237cc0,&UNK_180237fd8);
    }
  }
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_1c8);
  return;
}

