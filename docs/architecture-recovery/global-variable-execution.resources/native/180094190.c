
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180094190(undefined8 param_1,undefined4 param_2,undefined8 *param_3,undefined4 param_4,
                  longlong *param_5,undefined1 param_6)

{
  char cVar1;
  undefined4 uVar2;
  undefined8 uVar3;
  undefined1 auStack_198 [32];
  undefined4 uStack_178;
  undefined8 *puStack_170;
  undefined8 *puStack_168;
  undefined8 *puStack_160;
  undefined8 *puStack_158;
  undefined8 uStack_148;
  undefined8 auStack_140 [3];
  ulonglong uStack_128;
  undefined8 auStack_120 [3];
  ulonglong uStack_108;
  undefined8 auStack_100 [3];
  ulonglong uStack_e8;
  undefined8 auStack_e0 [3];
  ulonglong uStack_c8;
  undefined8 auStack_c0 [3];
  ulonglong uStack_a8;
  undefined8 auStack_a0 [3];
  ulonglong uStack_88;
  undefined8 auStack_80 [3];
  ulonglong uStack_68;
  undefined8 auStack_60 [3];
  ulonglong uStack_48;
  ulonglong uStack_40;
  
  uStack_148 = 0xfffffffffffffffe;
  uStack_40 = _DAT_1802a5230 ^ (ulonglong)auStack_198;
  if (*param_5 == 0) {
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 != '\0') {
      if (0xf < (ulonglong)param_3[3]) {
        param_3 = (undefined8 *)*param_3;
      }
      uVar3 = FUN_18001db50();
      uStack_178 = 0x69;
      puStack_170 = param_3;
      FUN_180015b40(uVar3,0,&UNK_1802246f0,&UNK_180225cc8);
    }
    goto LAB_1800946c3;
  }
  switch(param_4) {
  case 1:
    uStack_178 = CONCAT31(uStack_178._1_3_,param_6);
    uVar2 = FUN_180094700(param_1,param_2,param_3);
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 == '\0') goto LAB_1800946c3;
    puStack_160 = (undefined8 *)FUN_18008f750(auStack_c0,uVar2);
    if (0xf < (ulonglong)puStack_160[3]) {
      puStack_160 = (undefined8 *)*puStack_160;
    }
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (undefined8 *)*param_3;
    }
    uVar3 = FUN_18001db50();
    puStack_170 = (undefined8 *)CONCAT44(puStack_170._4_4_,param_2);
    uStack_178 = 0x74;
    puStack_168 = param_3;
    FUN_180015b40(uVar3,0,&UNK_180225c80,&UNK_180225cc8);
    auStack_100[0] = auStack_c0[0];
    uStack_e8 = uStack_a8;
    goto joined_r0x000180094353;
  case 2:
    uStack_178 = CONCAT31(uStack_178._1_3_,param_6);
    uVar2 = FUN_180094c80(param_1,param_2,param_3);
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 == '\0') goto LAB_1800946c3;
    puStack_160 = (undefined8 *)FUN_18008f750(auStack_100,uVar2);
    if (0xf < (ulonglong)puStack_160[3]) {
      puStack_160 = (undefined8 *)*puStack_160;
    }
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (undefined8 *)*param_3;
    }
    uVar3 = FUN_18001db50();
    puStack_170 = (undefined8 *)CONCAT44(puStack_170._4_4_,param_2);
    uStack_178 = 0x78;
    puStack_168 = param_3;
    FUN_180015b40(uVar3,0,&UNK_180225d50,&UNK_180225cc8);
joined_r0x000180094353:
    if (uStack_e8 < 0x10) goto LAB_1800946c3;
    goto code_r0x0001800946bb;
  case 3:
    uStack_178 = CONCAT31(uStack_178._1_3_,param_6);
    uVar2 = FUN_1800957d0(param_1,param_2,param_3);
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 == '\0') goto LAB_1800946c3;
    puStack_160 = (undefined8 *)FUN_18008f750(auStack_140,uVar2);
    if (0xf < (ulonglong)puStack_160[3]) {
      puStack_160 = (undefined8 *)*puStack_160;
    }
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (undefined8 *)*param_3;
    }
    uVar3 = FUN_18001db50();
    puStack_170 = (undefined8 *)CONCAT44(puStack_170._4_4_,param_2);
    uStack_178 = 0x80;
    puStack_168 = param_3;
    FUN_180015b40(uVar3,0,&UNK_180225df0,&UNK_180225cc8);
    auStack_100[0] = auStack_140[0];
    uStack_48 = uStack_128;
    break;
  case 4:
    uStack_178 = CONCAT31(uStack_178._1_3_,param_6);
    uVar2 = FUN_180095f40(param_1,param_2,param_3);
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 == '\0') goto LAB_1800946c3;
    puStack_160 = (undefined8 *)FUN_18008f750(auStack_120,uVar2);
    if (0xf < (ulonglong)puStack_160[3]) {
      puStack_160 = (undefined8 *)*puStack_160;
    }
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (undefined8 *)*param_3;
    }
    uVar3 = FUN_18001db50();
    puStack_170 = (undefined8 *)CONCAT44(puStack_170._4_4_,param_2);
    uStack_178 = 0x84;
    puStack_168 = param_3;
    FUN_180015b40(uVar3,0,&UNK_180225c80,&UNK_180225cc8);
    auStack_100[0] = auStack_120[0];
    uStack_48 = uStack_108;
    break;
  case 5:
    uStack_178 = CONCAT31(uStack_178._1_3_,param_6);
    uVar2 = FUN_1800964e0(param_1,param_2,param_3);
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 == '\0') goto LAB_1800946c3;
    puStack_160 = (undefined8 *)FUN_18008f750(auStack_e0,uVar2);
    if (0xf < (ulonglong)puStack_160[3]) {
      puStack_160 = (undefined8 *)*puStack_160;
    }
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (undefined8 *)*param_3;
    }
    uVar3 = FUN_18001db50();
    puStack_170 = (undefined8 *)CONCAT44(puStack_170._4_4_,param_2);
    uStack_178 = 0x88;
    puStack_168 = param_3;
    FUN_180015b40(uVar3,0,&UNK_180225d50,&UNK_180225cc8);
    auStack_100[0] = auStack_e0[0];
    uStack_48 = uStack_c8;
    break;
  case 6:
    uStack_178 = CONCAT31(uStack_178._1_3_,param_6);
    uVar2 = FUN_180096a60(param_1,param_2,param_3);
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 == '\0') goto LAB_1800946c3;
    puStack_160 = (undefined8 *)FUN_18008f750(auStack_a0,uVar2);
    if (0xf < (ulonglong)puStack_160[3]) {
      puStack_160 = (undefined8 *)*puStack_160;
    }
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (undefined8 *)*param_3;
    }
    uVar3 = FUN_18001db50();
    puStack_170 = (undefined8 *)CONCAT44(puStack_170._4_4_,param_2);
    uStack_178 = 0x8c;
    puStack_168 = param_3;
    FUN_180015b40(uVar3,0,&UNK_180225df0,&UNK_180225cc8);
    auStack_100[0] = auStack_a0[0];
    uStack_48 = uStack_88;
    break;
  case 7:
    uStack_178 = CONCAT31(uStack_178._1_3_,param_6);
    uVar2 = FUN_180095230(param_1,param_2,param_3);
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 == '\0') goto LAB_1800946c3;
    puStack_160 = (undefined8 *)FUN_18008f750(auStack_80,uVar2);
    if (0xf < (ulonglong)puStack_160[3]) {
      puStack_160 = (undefined8 *)*puStack_160;
    }
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (undefined8 *)*param_3;
    }
    uVar3 = FUN_18001db50();
    puStack_170 = (undefined8 *)CONCAT44(puStack_170._4_4_,param_2);
    uStack_178 = 0x7c;
    puStack_168 = param_3;
    FUN_180015b40(uVar3,0,&UNK_180225d00,&UNK_180225cc8);
    auStack_100[0] = auStack_80[0];
    uStack_48 = uStack_68;
    break;
  default:
    cVar1 = FUN_18001dbc0(4);
    if (cVar1 == '\0') goto LAB_1800946c3;
    puStack_158 = (undefined8 *)FUN_18008f750(auStack_60,0xe0000001);
    if (0xf < (ulonglong)puStack_158[3]) {
      puStack_158 = (undefined8 *)*puStack_158;
    }
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (undefined8 *)*param_3;
    }
    uVar3 = FUN_18001db50();
    puStack_168 = (undefined8 *)CONCAT44(puStack_168._4_4_,param_4);
    puStack_170 = (undefined8 *)CONCAT44(puStack_170._4_4_,param_2);
    uStack_178 = 0x90;
    puStack_160 = param_3;
    FUN_180015b40(uVar3,4,&UNK_180225da0,&UNK_180225cc8);
    auStack_100[0] = auStack_60[0];
  }
  if (0xf < uStack_48) {
code_r0x0001800946bb:
    __3_YAXPEAX_Z(auStack_100[0]);
  }
LAB_1800946c3:
  FUN_1801e8810(uStack_40 ^ (ulonglong)auStack_198);
  return;
}

