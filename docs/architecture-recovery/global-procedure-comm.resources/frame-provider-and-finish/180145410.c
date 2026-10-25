
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_SetModuleParamBinding(longlong param_1,int param_2)

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
  undefined1 uStack_178;
  undefined7 uStack_177;
  undefined8 uStack_168;
  ulonglong uStack_160;
  undefined1 auStack_158 [272];
  ulonglong uStack_48;
  
                    /* 0x145410  217  VM_M_SetModuleParamBinding */
  uStack_48 = _DAT_1802a5230 ^ (ulonglong)auStack_1c8;
  if (DAT_1802b129e == '\0') {
    if (param_1 != 0) {
      cVar1 = FUN_18001dbc0(2);
      if (cVar1 != '\0') {
        uVar3 = FUN_18001db50();
        puStack_1a0 = (undefined8 *)CONCAT44(puStack_1a0._4_4_,param_2);
        puStack_1a8 = (undefined1 *)CONCAT44(puStack_1a8._4_4_,0x9cf);
        FUN_180015b40(uVar3,2,&UNK_180238bb8,&UNK_180238b98);
      }
      iVar2 = 0;
      iVar8 = 0;
      lVar9 = 0;
      if (0 < param_2) {
        pcVar6 = (char *)(param_1 + 4);
        do {
          lVar5 = (longlong)iVar8 * 0x298 + param_1;
          lVar7 = lVar5 + 0x88;
          FUN_180143f80(lVar7,auStack_158);
          cVar1 = FUN_18001dbc0(2);
          if (cVar1 != '\0') {
            puVar4 = (undefined8 *)FUN_180107380(&uStack_178,*(undefined4 *)(pcVar6 + 0x80));
            if (0xf < (ulonglong)puVar4[3]) {
              puVar4 = (undefined8 *)*puVar4;
            }
            uVar3 = FUN_18001db50();
            uStack_198 = *(undefined4 *)(pcVar6 + -4);
            puStack_1a0 = (undefined8 *)CONCAT44(puStack_1a0._4_4_,iVar8);
            puStack_1a8 = (undefined1 *)CONCAT44(puStack_1a8._4_4_,0x9d8);
            lStack_190 = lVar5 + 4;
            puStack_188 = puVar4;
            FUN_180015b40(uVar3,2,&UNK_18022ad58,&UNK_180238b98);
            if (0xf < uStack_160) {
              __3_YAXPEAX_Z(CONCAT71(uStack_177,uStack_178));
            }
          }
          cVar1 = FUN_18001dbc0(2);
          if (cVar1 != '\0') {
            puVar4 = (undefined8 *)FUN_180107170(&uStack_178,lVar7);
            if (0xf < (ulonglong)puVar4[3]) {
              puVar4 = (undefined8 *)*puVar4;
            }
            uVar3 = FUN_18001db50();
            puStack_1a8 = (undefined1 *)CONCAT44(puStack_1a8._4_4_,0x9d9);
            puStack_1a0 = puVar4;
            FUN_180015b40(uVar3,2,&UNK_18021fa18,&UNK_180238b98);
            if (0xf < uStack_160) {
              __3_YAXPEAX_Z(CONCAT71(uStack_177,uStack_178));
            }
          }
          uStack_160 = 0xf;
          uStack_168 = 0;
          uStack_178 = 0;
          if (*pcVar6 == '\0') {
            lVar7 = 0;
          }
          else {
            lVar7 = -1;
            do {
              lVar7 = lVar7 + 1;
            } while (pcVar6[lVar7] != '\0');
          }
          FUN_180006300(&uStack_178,pcVar6,lVar7);
          if ((_DAT_1802b30c4 & 1) == 0) {
            _DAT_1802b30c4 = _DAT_1802b30c4 | 1;
            FUN_1800bcee0();
            FUN_1801e8930(&UNK_180202f20);
          }
          puStack_1a8 = auStack_158;
          iVar2 = FUN_1800bd110(&DAT_1802b30d0,*(undefined4 *)(pcVar6 + -4),&uStack_178,
                                *(undefined4 *)(pcVar6 + 0x80));
          if (0xf < uStack_160) {
            __3_YAXPEAX_Z(CONCAT71(uStack_177,uStack_178));
          }
          if (iVar2 != 0) break;
          lVar9 = lVar9 + 1;
          iVar8 = iVar8 + 1;
          pcVar6 = pcVar6 + 0x298;
        } while (lVar9 < param_2);
      }
      cVar1 = FUN_18001dbc0(2);
      if (cVar1 != '\0') {
        puVar4 = (undefined8 *)FUN_18008f750(&uStack_178,iVar2);
        if (0xf < (ulonglong)puVar4[3]) {
          puVar4 = (undefined8 *)*puVar4;
        }
        uVar3 = FUN_18001db50();
        puStack_1a8 = (undefined1 *)CONCAT44(puStack_1a8._4_4_,0x9e7);
        puStack_1a0 = puVar4;
        FUN_180015b40(uVar3,2,&UNK_180234258,&UNK_180238b98);
        if (0xf < uStack_160) {
          __3_YAXPEAX_Z(CONCAT71(uStack_177,uStack_178));
        }
      }
    }
  }
  else {
    cVar1 = FUN_18001dbc0(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_18001db50();
      puStack_1a8 = (undefined1 *)CONCAT44(puStack_1a8._4_4_,0x9c8);
      FUN_180015b40(uVar3,4,&UNK_180237cc0,&UNK_180238b98);
    }
  }
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_1c8);
  return;
}

