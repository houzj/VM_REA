
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180670200(undefined8 param_1,undefined4 param_2,undefined4 param_3,undefined8 **param_4)

{
  undefined8 *puVar1;
  longlong lVar2;
  undefined4 uVar3;
  undefined4 uVar4;
  undefined4 uVar5;
  char cVar6;
  longlong *plVar7;
  undefined4 *puVar8;
  ulonglong uVar9;
  undefined8 uVar10;
  uint uVar11;
  longlong *plVar12;
  uint uVar13;
  uint uVar14;
  longlong *plVar15;
  undefined1 auStack_198 [32];
  undefined4 uStack_178;
  undefined *puStack_170;
  undefined *puStack_168;
  undefined8 uStack_160;
  undefined8 uStack_158;
  undefined8 uStack_150;
  char *pcStack_148;
  undefined1 *puStack_140;
  longlong lStack_138;
  undefined8 uStack_130;
  undefined8 uStack_128;
  undefined8 uStack_120;
  undefined8 uStack_118;
  undefined8 uStack_110;
  undefined8 uStack_108;
  undefined8 *puStack_f8;
  undefined8 uStack_f0;
  undefined8 uStack_e8;
  ulonglong uStack_e0;
  undefined8 uStack_d8;
  undefined8 uStack_d0;
  undefined8 uStack_c8;
  undefined4 uStack_c0;
  undefined1 uStack_bc;
  undefined8 uStack_b8;
  ulonglong uStack_b0;
  longlong *plStack_a8;
  longlong *plStack_a0;
  undefined8 uStack_98;
  undefined8 uStack_90;
  undefined8 uStack_88;
  undefined8 uStack_80;
  undefined8 uStack_78;
  undefined8 uStack_70;
  undefined4 uStack_68;
  undefined8 uStack_60;
  undefined8 uStack_58;
  ulonglong uStack_50;
  ulonglong uStack_48;
  
  uStack_128 = 0xfffffffffffffffe;
  uStack_48 = _DAT_1808559e0 ^ (ulonglong)auStack_198;
  uStack_b8 = 0;
  plVar15 = (longlong *)0x0;
  uStack_b0 = 0;
  plStack_a8 = (longlong *)0x0;
  plStack_a0 = (longlong *)0x0;
  uStack_98 = 0;
  uStack_90 = 0;
  uStack_88 = 0;
  uStack_80 = 0;
  uStack_78 = 0;
  uStack_70 = 0x400;
  uStack_68 = 0;
  uStack_60 = 0;
  plVar7 = (longlong *)__2_YAPEAX_K_Z(0x28);
  plVar12 = plVar15;
  if (plVar7 != (longlong *)0x0) {
    *plVar7 = 0;
    plVar7[1] = 0x10000;
    plVar7[2] = 0;
    plVar7[3] = 0;
    plVar7[4] = 0;
    plVar12 = plVar7;
  }
  uStack_b8 = 0;
  uStack_b0 = 0x3000000000000;
  uStack_120 = 0;
  uStack_118 = 0x3000000000000;
  puStack_168 = &UNK_1807d7650;
  uStack_160 = CONCAT44(uStack_160._4_4_,7);
  plStack_a8 = plVar12;
  plStack_a0 = plVar12;
  FUN_180697150(&uStack_120,&puStack_168,0xbc9,plVar12);
  uStack_108 = 0xc05000000000000;
  uStack_110 = 0x16;
  puVar8 = (undefined4 *)FUN_1805fdc00(plVar12,0x17);
  uVar5 = _UNK_1807e7a74;
  uVar4 = _UNK_1807e7a70;
  uVar3 = _UNK_1807e7a6c;
  uStack_108 = uStack_108 & 0xffff000000000000 | (ulonglong)puVar8;
  *puVar8 = _DAT_1807e7a68;
  puVar8[1] = uVar3;
  puVar8[2] = uVar4;
  puVar8[3] = uVar5;
  puVar8[4] = _DAT_1807e7a78;
  *(undefined2 *)(puVar8 + 5) = _DAT_1807e7a7c;
  *(undefined1 *)((longlong)puVar8 + 0x16) = 0;
  uStack_160 = 0x4050001807e72b0;
  puStack_168 = (undefined *)0xb;
  uVar14 = 0x10;
  uVar11 = (uint)uStack_120;
  if (uStack_120._4_4_ <= (uint)uStack_120) {
    if (uStack_120._4_4_ == 0) {
      uVar13 = 0x10;
    }
    else {
      uVar13 = (uStack_120._4_4_ + 1 >> 1) + uStack_120._4_4_;
      if (uVar13 <= uStack_120._4_4_) goto LAB_180670404;
    }
    uVar9 = FUN_180697080(plVar12,uStack_118 & 0xffffffffffff,(ulonglong)uStack_120._4_4_ << 5,
                          (ulonglong)uVar13 << 5);
    uStack_118 = uStack_118 & 0xffff000000000000 | uVar9;
    uStack_120 = CONCAT44(uVar13,(uint)uStack_120);
    uVar11 = (uint)uStack_120;
  }
LAB_180670404:
  puVar1 = (undefined8 *)((ulonglong)uVar11 * 0x20 + (uStack_118 & 0xffffffffffff));
  *puVar1 = puStack_168;
  puVar1[1] = uStack_160;
  puVar8 = (undefined4 *)((uStack_120 & 0xffffffff) * 0x20 + 0x10 + (uStack_118 & 0xffffffffffff));
  *puVar8 = (undefined4)uStack_110;
  puVar8[1] = uStack_110._4_4_;
  puVar8[2] = (undefined4)uStack_108;
  puVar8[3] = uStack_108._4_4_;
  uStack_108 = uStack_108 & 0xffffffffffff;
  uStack_120 = CONCAT44(uStack_120._4_4_,(uint)uStack_120 + 1);
  puStack_168 = &UNK_1807e72c0;
  uStack_160 = CONCAT44(uStack_160._4_4_,7);
  puStack_f8 = (undefined8 *)&UNK_1807e71c4;
  uStack_f0._0_4_ = 4;
  FUN_180696d80(&uStack_120,&puStack_f8,&puStack_168,plVar12);
  puStack_f8 = (undefined8 *)&UNK_1807e7128;
  uStack_f0 = CONCAT44(uStack_f0._4_4_,9);
  FUN_180697150(&uStack_120,&puStack_f8,param_3,plVar12);
  uStack_160 = 0x4050001807d7690;
  puStack_168 = (undefined *)0x4;
  uVar11 = (uint)uStack_b8;
  if (uStack_b8._4_4_ <= (uint)uStack_b8) {
    if (uStack_b8._4_4_ == 0) {
      uVar13 = 0x10;
    }
    else {
      uVar13 = (uStack_b8._4_4_ + 1 >> 1) + uStack_b8._4_4_;
      if (uVar13 <= uStack_b8._4_4_) goto LAB_180670539;
    }
    uVar9 = FUN_180697080(plVar12,uStack_b0 & 0xffffffffffff,(ulonglong)uStack_b8._4_4_ << 5,
                          (ulonglong)uVar13 << 5);
    uStack_b0 = uStack_b0 & 0xffff000000000000 | uVar9;
    uStack_b8 = CONCAT44(uVar13,(uint)uStack_b8);
    uVar11 = (uint)uStack_b8;
  }
LAB_180670539:
  puVar1 = (undefined8 *)((ulonglong)uVar11 * 0x20 + (uStack_b0 & 0xffffffffffff));
  *puVar1 = puStack_168;
  puVar1[1] = uStack_160;
  puVar8 = (undefined4 *)((uStack_b8 & 0xffffffff) * 0x20 + 0x10 + (uStack_b0 & 0xffffffffffff));
  *puVar8 = (uint)uStack_120;
  puVar8[1] = uStack_120._4_4_;
  puVar8[2] = (undefined4)uStack_118;
  puVar8[3] = uStack_118._4_4_;
  uStack_118 = uStack_118 & 0xffffffffffff;
  uStack_58 = 0;
  uStack_50 = 0x3000000000000;
  puStack_f8 = (undefined8 *)&UNK_1807e70e8;
  uStack_f0 = CONCAT44(uStack_f0._4_4_,0xb);
  uStack_b8._0_4_ = (uint)uStack_b8 + 1;
  FUN_180697150(&uStack_58,&puStack_f8,param_2,plVar12);
  uStack_160 = 0x4050001807dd648;
  puStack_168 = (undefined *)0x4;
  if ((uStack_b8._4_4_ <= (uint)uStack_b8) &&
     ((uStack_b8._4_4_ == 0 ||
      (uVar14 = (uStack_b8._4_4_ + 1 >> 1) + uStack_b8._4_4_, uStack_b8._4_4_ < uVar14)))) {
    uVar9 = FUN_180697080(plVar12,uStack_b0 & 0xffffffffffff,(ulonglong)uStack_b8._4_4_ << 5,
                          (ulonglong)uVar14 << 5);
    uStack_b0 = uStack_b0 & 0xffff000000000000 | uVar9;
    uStack_b8._4_4_ = uVar14;
  }
  puVar1 = (undefined8 *)((ulonglong)(uint)uStack_b8 * 0x20 + (uStack_b0 & 0xffffffffffff));
  *puVar1 = puStack_168;
  puVar1[1] = uStack_160;
  puVar1 = (undefined8 *)((ulonglong)(uint)uStack_b8 * 0x20 + 0x10 + (uStack_b0 & 0xffffffffffff));
  *puVar1 = uStack_58;
  puVar1[1] = uStack_50;
  uStack_50 = uStack_50 & 0xffffffffffff;
  uStack_b8 = CONCAT44(uStack_b8._4_4_,(uint)uStack_b8 + 1);
  uStack_158 = 0;
  uStack_150 = 0;
  pcStack_148 = (char *)0x0;
  puStack_140 = (undefined1 *)0x0;
  lStack_138 = 0;
  uStack_130 = 0x100;
  puStack_f8 = &uStack_158;
  uStack_f0 = 0;
  uStack_e8 = 0;
  uStack_e0 = 0;
  uStack_d8 = 0;
  uStack_d0 = 0;
  uStack_c8 = 0x200;
  uStack_c0 = 0x144;
  uStack_bc = 0;
  cVar6 = FUN_18063b3c0(&uStack_b8,&puStack_f8);
  if (cVar6 == '\0') {
    free();
    __3_YAXPEAX_Z(uStack_e8);
    cVar6 = FUN_18057a5b0(4);
    if (cVar6 != '\0') {
      uVar10 = FUN_18057a310();
      puStack_170 = &UNK_1807e7378;
      uStack_178 = 0x3f9;
      FUN_18057a4b0(uVar10,4,&UNK_1807cc748,&UNK_1807e7a80);
    }
  }
  else {
    free(uStack_e0);
    __3_YAXPEAX_Z(uStack_e8);
    if (lStack_138 - (longlong)puStack_140 < 1) {
      FUN_1805fbdb0(&uStack_158,1);
    }
    *puStack_140 = 0;
    uStack_e0 = 0xf;
    uStack_e8 = 0;
    puStack_f8 = (undefined8 *)((ulonglong)puStack_f8 & 0xffffffffffffff00);
    if (*pcStack_148 != '\0') {
      plVar15 = (longlong *)0xffffffffffffffff;
      do {
        plVar15 = (longlong *)((longlong)plVar15 + 1);
      } while (pcStack_148[(longlong)plVar15] != '\0');
    }
    FUN_180577de0(&puStack_f8,pcStack_148,plVar15);
    if (param_4 != &puStack_f8) {
      if ((undefined8 *)0xf < param_4[3]) {
        __3_YAXPEAX_Z(*param_4);
      }
      param_4[3] = (undefined8 *)0xf;
      param_4[2] = (undefined8 *)0x0;
      *(undefined1 *)param_4 = 0;
      FUN_180581c90(param_4,&puStack_f8);
    }
    if (0xf < uStack_e0) {
      __3_YAXPEAX_Z(puStack_f8);
    }
  }
  free(pcStack_148);
  __3_YAXPEAX_Z(uStack_150);
  plVar12 = plStack_a0;
  if (plStack_a0 != (longlong *)0x0) {
    if (*plStack_a0 == 0) {
LAB_180670822:
      lVar2 = *plVar12;
      if ((lVar2 != 0) && (lVar2 == plVar12[2])) {
        *(undefined8 *)(lVar2 + 8) = 0;
      }
    }
    else {
      do {
        if (*plVar12 == plVar12[2]) goto LAB_180670822;
        lVar2 = *(longlong *)(*plVar12 + 0x10);
        free(*plVar12);
        *plVar12 = lVar2;
      } while (lVar2 != 0);
    }
    __3_YAXPEAX_Z(plVar12[4]);
    __3_YAXPEAX_Z(plVar12);
  }
  free(uStack_88);
  __3_YAXPEAX_Z(uStack_90);
  FUN_180745b60(uStack_48 ^ (ulonglong)auStack_198);
  return;
}

