
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800a7830(longlong param_1,longlong *param_2,uint *param_3)

{
  uint uVar1;
  undefined8 *puVar2;
  undefined8 *puVar3;
  longlong lVar4;
  ulonglong uVar5;
  char cVar6;
  int iVar7;
  longlong *plVar8;
  undefined8 *puVar9;
  undefined8 uVar10;
  longlong *plVar11;
  longlong *plVar12;
  bool bVar13;
  undefined1 auStack_1b8 [32];
  undefined4 uStack_198;
  undefined4 uStack_190;
  undefined8 uStack_188;
  longlong *plStack_180;
  longlong *plStack_178;
  undefined8 *puStack_170;
  int iStack_168;
  longlong lStack_160;
  undefined4 uStack_158;
  uint *puStack_150;
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
  uStack_40 = DAT_1802a5230 ^ (ulonglong)auStack_1b8;
  iStack_168 = 0;
  puVar2 = (undefined8 *)*param_2;
  puVar3 = (undefined8 *)*puVar2;
  puStack_150 = param_3;
  lStack_160 = param_1;
  do {
    if (puVar3 == puVar2) {
      FUN_1801e8810(uStack_40 ^ (ulonglong)auStack_1b8);
      return;
    }
    uStack_158 = *(undefined4 *)(puVar3 + 2);
    plVar12 = puVar3 + 3;
    bVar13 = *(int *)((longlong)puVar3 + 0x14) == 1;
    switch(uStack_158) {
    case 1:
      if (bVar13) {
        (**(code **)(*(longlong *)puVar3[7] + 8))();
      }
      iVar7 = FUN_180013450(param_1 + 0x128,puVar3 + 2);
      cVar6 = FUN_18001dbc0(0);
      if (cVar6 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_100,iVar7);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = plVar12;
        if (0xf < (ulonglong)puVar3[6]) {
          plVar11 = (longlong *)*plVar12;
        }
        uVar10 = FUN_18001db50();
        param_1 = lStack_160;
        uStack_188 = *(undefined8 *)(lStack_160 + 0xf8);
        uStack_190 = *(undefined4 *)(lStack_160 + 0x2c);
        uStack_198 = 0x5f9;
        plStack_180 = plVar11;
        plStack_178 = plVar8;
        FUN_180015b40(uVar10,0,&DAT_1802282f8,&DAT_180228260);
        uVar10 = auStack_100[0];
        uVar5 = uStack_e8;
joined_r0x0001800a7ade:
        if (0xf < uVar5) {
          __3_YAXPEAX_Z(uVar10);
        }
      }
      break;
    case 2:
      if (bVar13) {
        (**(code **)(*(longlong *)puVar3[9] + 8))();
      }
      iVar7 = FUN_1800139f0(param_1 + 0x128,puVar3 + 2);
      cVar6 = FUN_18001dbc0(0);
      if (cVar6 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_140,iVar7);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = plVar12;
        if (0xf < (ulonglong)puVar3[6]) {
          plVar11 = (longlong *)*plVar12;
        }
        uVar10 = FUN_18001db50();
        param_1 = lStack_160;
        uStack_188 = *(undefined8 *)(lStack_160 + 0xf8);
        uStack_190 = *(undefined4 *)(lStack_160 + 0x2c);
        uStack_198 = 0x5fe;
        plStack_180 = plVar11;
        plStack_178 = plVar8;
        FUN_180015b40(uVar10,0,&DAT_1802282b8,&DAT_180228260);
        uVar10 = auStack_140[0];
        uVar5 = uStack_128;
        goto joined_r0x0001800a7ade;
      }
      break;
    case 3:
      if (bVar13) {
        (**(code **)(*(longlong *)puVar3[0xb] + 8))();
      }
      iVar7 = FUN_180013fa0(param_1 + 0x128,puVar3 + 2);
      cVar6 = FUN_18001dbc0(0);
      if (cVar6 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_c0,iVar7);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = plVar12;
        if (0xf < (ulonglong)puVar3[6]) {
          plVar11 = (longlong *)*plVar12;
        }
        uVar10 = FUN_18001db50();
        param_1 = lStack_160;
        uStack_188 = *(undefined8 *)(lStack_160 + 0xf8);
        uStack_190 = *(undefined4 *)(lStack_160 + 0x2c);
        uStack_198 = 0x608;
        plStack_180 = plVar11;
        plStack_178 = plVar8;
        FUN_180015b40(uVar10,0,&DAT_180228378,&DAT_180228260);
        uVar10 = auStack_c0[0];
        uVar5 = uStack_a8;
        goto joined_r0x0001800a7ade;
      }
      break;
    case 4:
      (**(code **)(*(longlong *)puVar3[0xd] + 8))();
      iVar7 = FUN_1800145f0(param_1 + 0x128,puVar3 + 2);
      cVar6 = FUN_18001dbc0(0);
      if (cVar6 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_120,iVar7);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = plVar12;
        if (0xf < (ulonglong)puVar3[6]) {
          plVar11 = (longlong *)*plVar12;
        }
        uVar10 = FUN_18001db50();
        param_1 = lStack_160;
        uStack_188 = *(undefined8 *)(lStack_160 + 0xf8);
        uStack_190 = *(undefined4 *)(lStack_160 + 0x2c);
        uStack_198 = 0x60d;
        plStack_180 = plVar11;
        plStack_178 = plVar8;
        FUN_180015b40(uVar10,0,&DAT_180228338,&DAT_180228260);
        uVar10 = auStack_120[0];
        uVar5 = uStack_108;
        goto joined_r0x0001800a7ade;
      }
      break;
    case 5:
      (**(code **)(*(longlong *)puVar3[0xf] + 8))();
      iVar7 = FUN_180014ae0(param_1 + 0x128,puVar3 + 2);
      cVar6 = FUN_18001dbc0(0);
      if (cVar6 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_e0,iVar7);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = plVar12;
        if (0xf < (ulonglong)puVar3[6]) {
          plVar11 = (longlong *)*plVar12;
        }
        uVar10 = FUN_18001db50();
        param_1 = lStack_160;
        uStack_188 = *(undefined8 *)(lStack_160 + 0xf8);
        uStack_190 = *(undefined4 *)(lStack_160 + 0x2c);
        uStack_198 = 0x612;
        plStack_180 = plVar11;
        plStack_178 = plVar8;
        FUN_180015b40(uVar10,0,&DAT_1802283f8,&DAT_180228260);
        uVar10 = auStack_e0[0];
        uVar5 = uStack_c8;
        goto joined_r0x0001800a7ade;
      }
      break;
    case 6:
      if (bVar13) {
        (**(code **)(*(longlong *)puVar3[0x11] + 8))();
      }
      iVar7 = FUN_180014fd0(param_1 + 0x128,puVar3 + 2);
      cVar6 = FUN_18001dbc0(0);
      if (cVar6 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_a0,iVar7);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = plVar12;
        if (0xf < (ulonglong)puVar3[6]) {
          plVar11 = (longlong *)*plVar12;
        }
        uVar10 = FUN_18001db50();
        param_1 = lStack_160;
        uStack_188 = *(undefined8 *)(lStack_160 + 0xf8);
        uStack_190 = *(undefined4 *)(lStack_160 + 0x2c);
        uStack_198 = 0x617;
        plStack_180 = plVar11;
        plStack_178 = plVar8;
        FUN_180015b40(uVar10,0,&DAT_1802283b8,&DAT_180228260);
        uVar10 = auStack_a0[0];
        uVar5 = uStack_88;
        goto joined_r0x0001800a7ade;
      }
      break;
    case 7:
      if (bVar13) {
        (**(code **)(*(longlong *)puVar3[9] + 8))();
      }
      iVar7 = FUN_180015440(param_1 + 0x128,puVar3 + 2);
      cVar6 = FUN_18001dbc0(0);
      if (cVar6 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_80,iVar7);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = plVar12;
        if (0xf < (ulonglong)puVar3[6]) {
          plVar11 = (longlong *)*plVar12;
        }
        uVar10 = FUN_18001db50();
        param_1 = lStack_160;
        uStack_188 = *(undefined8 *)(lStack_160 + 0xf8);
        uStack_190 = *(undefined4 *)(lStack_160 + 0x2c);
        uStack_198 = 0x603;
        plStack_180 = plVar11;
        plStack_178 = plVar8;
        FUN_180015b40(uVar10,0,&DAT_1802282b8,&DAT_180228260);
        uVar10 = auStack_80[0];
        uVar5 = uStack_68;
        goto joined_r0x0001800a7ade;
      }
      break;
    default:
      iVar7 = -0x1fffffff;
      cVar6 = FUN_18001dbc0(4);
      if (cVar6 != '\0') {
        puVar9 = (undefined8 *)FUN_18008f750(auStack_60,0xe0000001);
        if (0xf < (ulonglong)puVar9[3]) {
          puVar9 = (undefined8 *)*puVar9;
        }
        plVar8 = plVar12;
        if (0xf < (ulonglong)puVar3[6]) {
          plVar8 = (longlong *)*plVar12;
        }
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
        param_1 = lStack_160;
        plStack_180 = (longlong *)CONCAT44(plStack_180._4_4_,uStack_158);
        uStack_188 = *(undefined8 *)(lStack_160 + 0xf8);
        uStack_190 = *(undefined4 *)(lStack_160 + 0x2c);
        uStack_198 = 0x61b;
        plStack_178 = plVar8;
        puStack_170 = puVar9;
        FUN_180015b40(&DAT_1802b1230,4,&DAT_180228480,&DAT_180228260);
        if (0xf < uStack_48) {
          __3_YAXPEAX_Z(auStack_60[0]);
        }
      }
      goto LAB_1800a7ec1;
    }
    if (iVar7 != 0) {
LAB_1800a7ec1:
      if (iStack_168 == 0) {
        cVar6 = FUN_18001dbc0(1);
        if (cVar6 != '\0') {
          plVar8 = plVar12;
          if (0xf < (ulonglong)puVar3[6]) {
            plVar8 = (longlong *)*plVar12;
          }
          uVar10 = FUN_18001db50();
          puStack_170 = (undefined8 *)CONCAT44(puStack_170._4_4_,iVar7);
          plStack_180 = (longlong *)CONCAT44(plStack_180._4_4_,uStack_158);
          uStack_188 = *(undefined8 *)(param_1 + 0xf8);
          uStack_190 = *(undefined4 *)(param_1 + 0x2c);
          uStack_198 = 0x622;
          plStack_178 = plVar8;
          FUN_180015b40(uVar10,1,&DAT_180228438,&DAT_180228260);
        }
        if (((*(int *)(puVar3 + 0x15) == 3) && (lVar4 = puVar3[0x2e], lVar4 != 0)) &&
           ((uVar1 = *(uint *)(lVar4 + 0x84), 6 < uVar1 || ((0x75U >> (uVar1 & 0x1f) & 1) == 0)))) {
          iVar7 = *(int *)(lVar4 + 0x7c);
          *puStack_150 = uVar1;
          iStack_168 = iVar7;
          cVar6 = FUN_18001dbc0(2);
          if (cVar6 != '\0') {
            if (0xf < (ulonglong)puVar3[6]) {
              plVar12 = (longlong *)*plVar12;
            }
            uVar10 = FUN_18001db50();
            puStack_170 = (undefined8 *)CONCAT44(puStack_170._4_4_,iVar7);
            plStack_178 = (longlong *)CONCAT44(plStack_178._4_4_,uVar1);
            uStack_188 = CONCAT44(uStack_188._4_4_,uStack_158);
            uStack_190 = *(undefined4 *)(param_1 + 0x2c);
            uStack_198 = 0x631;
            plStack_180 = plVar12;
            FUN_180015b40(uVar10,2,&DAT_180228500,&DAT_180228260);
          }
        }
      }
    }
    puVar3 = (undefined8 *)*puVar3;
  } while( true );
}

