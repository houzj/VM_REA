
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180035f60(longlong param_1,undefined8 param_2)

{
  undefined4 uVar1;
  code *pcVar2;
  char cVar3;
  int iVar4;
  undefined8 uVar5;
  longlong lVar6;
  longlong *plVar7;
  longlong *plVar8;
  undefined1 auStack_5f8 [32];
  undefined8 ***pppuStack_5d8;
  undefined1 *puStack_5d0;
  longlong lStack_5c8;
  int aiStack_5b8 [2];
  undefined1 uStack_5b0;
  undefined7 uStack_5af;
  undefined8 uStack_5a0;
  ulonglong uStack_598;
  undefined8 uStack_590;
  undefined1 uStack_588;
  undefined1 auStack_587 [511];
  undefined1 auStack_388 [512];
  undefined2 uStack_188;
  undefined1 uStack_186;
  undefined1 uStack_185;
  longlong *plStack_180;
  undefined1 uStack_178;
  undefined1 uStack_177;
  undefined4 uStack_174;
  undefined1 uStack_88;
  undefined7 uStack_87;
  undefined8 uStack_78;
  ulonglong uStack_70;
  undefined1 uStack_68;
  undefined7 uStack_67;
  undefined8 uStack_58;
  ulonglong uStack_50;
  undefined8 ***apppuStack_48 [3];
  ulonglong uStack_30;
  ulonglong uStack_28;
  
  uStack_590 = 0xfffffffffffffffe;
  uStack_28 = _DAT_180192230 ^ (ulonglong)auStack_5f8;
  cVar3 = FUN_18000c2c0(2);
  if (cVar3 != '\0') {
    uVar5 = FUN_18000bfe0();
    pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x247);
    FUN_18000c1c0(uVar5,2,&UNK_180149c80,&UNK_180149ce0);
  }
  FUN_1800115c0(apppuStack_48,param_2,&UNK_1801497a0);
  plVar7 = (longlong *)(param_1 + 0x118);
  uVar5 = FUN_1800115c0(&uStack_5b0,plVar7,&UNK_1801466d8);
  FUN_1800113a0(&uStack_88,uVar5,&UNK_180149ca8);
  if (0xf < uStack_598) {
    __3_YAXPEAX_Z(CONCAT71(uStack_5af,uStack_5b0));
  }
  uStack_598 = 0xf;
  uStack_5a0 = 0;
  uStack_5b0 = 0;
  FUN_1800115c0(&uStack_68,plVar7,&UNK_1801466d8);
  iVar4 = FUN_18003a660();
  cVar3 = FUN_18000c2c0(2);
  if (cVar3 != '\0') {
    uVar5 = FUN_18000bfe0();
    pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x24f);
    FUN_18000c1c0(uVar5,2,&UNK_180149d58,&UNK_180149ce0);
  }
  if ((iVar4 != 0) && (cVar3 = FUN_18000c2c0(3), cVar3 != '\0')) {
    uVar5 = FUN_18000bfe0();
    puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,iVar4);
    pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x252);
    FUN_18000c1c0(uVar5,3,&UNK_180149d70,&UNK_180149ce0);
  }
  iVar4 = FUN_180036ab0(param_1);
  if (iVar4 == 0) {
    cVar3 = FUN_18000c2c0(1);
    if (cVar3 != '\0') {
      uVar5 = FUN_18000bfe0();
      pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x25d);
      FUN_18000c1c0(uVar5,1,&UNK_180149d38,&UNK_180149ce0);
    }
    if (0xf < *(ulonglong *)(param_1 + 0x130)) {
      plVar7 = (longlong *)*plVar7;
    }
    if ((_DAT_180198030 & 1) == 0) {
      _DAT_180198030 = _DAT_180198030 | 1;
      func_0x00018011e414(&DAT_180198040,0,0x500);
      FUN_1800210b0();
      FUN_18011dbb0(&UNK_18012c170);
    }
    iVar4 = FUN_180021590(&DAT_180198040,plVar7);
    cVar3 = FUN_18000c2c0(1);
    if (cVar3 != '\0') {
      uVar5 = FUN_18000bfe0();
      pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x25f);
      FUN_18000c1c0(uVar5,1,&UNK_180149e10,&UNK_180149ce0);
    }
    if (iVar4 != 0) {
      cVar3 = FUN_18000c2c0(4);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,iVar4);
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x263);
        FUN_18000c1c0(uVar5,4,&UNK_180149e28,&UNK_180149ce0);
      }
      aiStack_5b8[0] = iVar4;
      func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
      pcVar2 = (code *)swi(3);
      (*pcVar2)();
      return;
    }
    uStack_588 = 0;
    func_0x00018011e414(auStack_587,0,0x1ff);
    func_0x00018011e414(auStack_388,0,0x300);
    pppuStack_5d8 = (undefined8 ***)&uStack_88;
    if (0xf < uStack_70) {
      pppuStack_5d8 = (undefined8 ***)CONCAT71(uStack_87,uStack_88);
    }
    _snprintf_s(&uStack_588,0x200,0x1ff,&UNK_18014647c);
    pppuStack_5d8 = apppuStack_48;
    if (0xf < uStack_30) {
      pppuStack_5d8 = apppuStack_48[0];
    }
    _snprintf_s(auStack_388,0x200,0x1ff,&UNK_18014647c);
    uStack_188 = 0xd01;
    uStack_186 = *(undefined1 *)(param_1 + 0x6c);
    plVar7 = (longlong *)(param_1 + 0x10);
    plStack_180 = plVar7;
    if (0xf < *(ulonglong *)(param_1 + 0x28)) {
      plStack_180 = (longlong *)*plVar7;
    }
    uStack_178 = *(char *)(param_1 + 400) != '\0';
    uStack_177 = *(undefined1 *)(param_1 + 0x90);
    uStack_174 = *(undefined4 *)(param_1 + 0x94);
    cVar3 = FUN_18000c2c0(1);
    if (cVar3 != '\0') {
      if (0xf < *(ulonglong *)(param_1 + 0x28)) {
        plVar7 = (longlong *)*plVar7;
      }
      uVar5 = FUN_18000bfe0();
      puStack_5d0 = &uStack_588;
      pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x273);
      lStack_5c8 = (longlong)plVar7;
      FUN_18000c1c0(uVar5,1,&UNK_180149da0,&UNK_180149ce0);
    }
    uStack_185 = 1;
    cVar3 = FUN_18000c2c0(2);
    if (cVar3 != '\0') {
      uVar5 = FUN_18000bfe0();
      pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x27b);
      FUN_18000c1c0(uVar5,2,&UNK_180149de0,&UNK_180149ce0);
    }
    lVar6 = FUN_180021530();
    iVar4 = (**(code **)(lVar6 + 8))(&uStack_588);
    if (iVar4 != 0) {
      cVar3 = FUN_18000c2c0(4);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,iVar4);
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x27f);
        FUN_18000c1c0(uVar5,4,&UNK_180149eb0,&UNK_180149ce0);
      }
      aiStack_5b8[0] = iVar4;
      func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
      pcVar2 = (code *)swi(3);
      (*pcVar2)();
      return;
    }
    cVar3 = FUN_18000c2c0(2);
    if (cVar3 != '\0') {
      uVar5 = FUN_18000bfe0();
      pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x282);
      FUN_18000c1c0(uVar5,2,&UNK_180149ee0,&UNK_180149ce0);
    }
    if (*(int *)(param_1 + 8) == 0) {
      lVar6 = FUN_180021530();
      iVar4 = (**(code **)(lVar6 + 0x2a8))(0);
      cVar3 = FUN_18000c2c0(2);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x288);
        FUN_18000c1c0(uVar5,2,&UNK_180149e50,&UNK_180149ce0);
      }
      lVar6 = FUN_18004a5b0();
      *(undefined1 *)(lVar6 + 0xc0) = 0;
    }
    else {
      lVar6 = FUN_180021530();
      iVar4 = (**(code **)(lVar6 + 0x2a8))(1);
      cVar3 = FUN_18000c2c0(2);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x28e);
        FUN_18000c1c0(uVar5,2,&UNK_180149e80,&UNK_180149ce0);
      }
      lVar6 = FUN_18004a5b0();
      *(undefined1 *)(lVar6 + 0xc0) = 1;
    }
    if (iVar4 != 0) {
      cVar3 = FUN_18000c2c0(4);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,iVar4);
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x294);
        FUN_18000c1c0(uVar5,4,&UNK_180149fb0,&UNK_180149ce0);
      }
      aiStack_5b8[0] = iVar4;
      func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
      pcVar2 = (code *)swi(3);
      (*pcVar2)();
      return;
    }
    iVar4 = *(int *)(param_1 + 0x7c);
    lVar6 = FUN_180021530();
    iVar4 = (**(code **)(lVar6 + 0x458))(iVar4 != 0);
    if (iVar4 != 0) {
      cVar3 = FUN_18000c2c0(4);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        lStack_5c8 = CONCAT44(lStack_5c8._4_4_,iVar4);
        puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,*(undefined4 *)(param_1 + 0x7c));
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x29c);
        FUN_18000c1c0(uVar5,4,&UNK_180149ff0,&UNK_180149ce0);
      }
      aiStack_5b8[0] = iVar4;
      func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
      pcVar2 = (code *)swi(3);
      (*pcVar2)();
      return;
    }
    uVar1 = *(undefined4 *)(param_1 + 0x80);
    lVar6 = FUN_180021530();
    iVar4 = (**(code **)(lVar6 + 0x470))(uVar1);
    if (iVar4 != 0) {
      cVar3 = FUN_18000c2c0(4);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        lStack_5c8 = CONCAT44(lStack_5c8._4_4_,iVar4);
        puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,*(undefined4 *)(param_1 + 0x80));
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x2a4);
        FUN_18000c1c0(uVar5,4,&UNK_180149f10,&UNK_180149ce0);
      }
      aiStack_5b8[0] = iVar4;
      func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
      pcVar2 = (code *)swi(3);
      (*pcVar2)();
      return;
    }
    plVar7 = (longlong *)(param_1 + 0x98);
    plVar8 = plVar7;
    if (0xf < *(ulonglong *)(param_1 + 0xb0)) {
      plVar8 = (longlong *)*plVar7;
    }
    lVar6 = FUN_180021530();
    iVar4 = (**(code **)(lVar6 + 0x460))(plVar8);
    if (iVar4 != 0) {
      cVar3 = FUN_18000c2c0(4);
      if (cVar3 != '\0') {
        if (0xf < *(ulonglong *)(param_1 + 0xb0)) {
          plVar7 = (longlong *)*plVar7;
        }
        uVar5 = FUN_18000bfe0();
        lStack_5c8 = CONCAT44(lStack_5c8._4_4_,iVar4);
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x2ac);
        puStack_5d0 = (undefined1 *)plVar7;
        FUN_18000c1c0(uVar5,4,&UNK_180149f60,&UNK_180149ce0);
      }
      aiStack_5b8[0] = iVar4;
      func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
      pcVar2 = (code *)swi(3);
      (*pcVar2)();
      return;
    }
    uVar1 = *(undefined4 *)(param_1 + 0x84);
    lVar6 = FUN_180021530();
    iVar4 = (**(code **)(lVar6 + 0x468))(uVar1);
    if (iVar4 != 0) {
      cVar3 = FUN_18000c2c0(4);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        lStack_5c8 = CONCAT44(lStack_5c8._4_4_,iVar4);
        puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,*(undefined4 *)(param_1 + 0x84));
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x2b4);
        FUN_18000c1c0(uVar5,4,&UNK_18014a0a0,&UNK_180149ce0);
      }
      aiStack_5b8[0] = iVar4;
      func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
      pcVar2 = (code *)swi(3);
      (*pcVar2)();
      return;
    }
    iVar4 = *(int *)(param_1 + 0x88);
    lVar6 = FUN_180021530();
    iVar4 = (**(code **)(lVar6 + 0x490))(iVar4 != 0);
    if (iVar4 != 0) {
      cVar3 = FUN_18000c2c0(4);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,*(undefined4 *)(param_1 + 0x88));
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,699);
        FUN_18000c1c0(uVar5,4,&UNK_18014a0f0,&UNK_180149ce0);
      }
      aiStack_5b8[0] = iVar4;
      func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
      pcVar2 = (code *)swi(3);
      (*pcVar2)();
      return;
    }
    lVar6 = FUN_180021530();
    iVar4 = (**(code **)(lVar6 + 0x220))(&UNK_180038030,param_1);
    if (iVar4 != 0) {
      cVar3 = FUN_18000c2c0(4);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,iVar4);
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x2c3);
        FUN_18000c1c0(uVar5,4,&UNK_18014a038,&UNK_180149ce0);
      }
      aiStack_5b8[0] = iVar4;
      func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
      pcVar2 = (code *)swi(3);
      (*pcVar2)();
      return;
    }
    lVar6 = FUN_180021530();
    iVar4 = (**(code **)(lVar6 + 0x248))(&UNK_180038050,param_1);
    if (iVar4 != 0) {
      cVar3 = FUN_18000c2c0(4);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,iVar4);
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x2cb);
        FUN_18000c1c0(uVar5,4,&UNK_18014a070,&UNK_180149ce0);
      }
      aiStack_5b8[0] = iVar4;
      func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
      pcVar2 = (code *)swi(3);
      (*pcVar2)();
      return;
    }
    lVar6 = FUN_180021530();
    iVar4 = (**(code **)(lVar6 + 0x250))(&UNK_180038070,param_1);
    if (iVar4 != 0) {
      cVar3 = FUN_18000c2c0(4);
      if (cVar3 != '\0') {
        uVar5 = FUN_18000bfe0();
        puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,iVar4);
        pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x2d2);
        FUN_18000c1c0(uVar5,4,&UNK_18014a070,&UNK_180149ce0);
      }
      aiStack_5b8[0] = iVar4;
      func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
      pcVar2 = (code *)swi(3);
      (*pcVar2)();
      return;
    }
    if (*(int *)(param_1 + 0x8c) == 0) {
      lVar6 = FUN_180021530();
      iVar4 = (**(code **)(lVar6 + 0x4a0))(&UNK_18014a188);
      if (iVar4 != 0) {
        cVar3 = FUN_18000c2c0(4);
        if (cVar3 != '\0') {
          uVar5 = FUN_18000bfe0();
          puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,iVar4);
          pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x2db);
          FUN_18000c1c0(uVar5,4,&UNK_18014a1a0,&UNK_180149ce0);
        }
        aiStack_5b8[0] = iVar4;
        func_0x00018011e41a(aiStack_5b8,&UNK_18018ea80);
        pcVar2 = (code *)swi(3);
        (*pcVar2)();
        return;
      }
    }
    cVar3 = FUN_18000c2c0(2);
    if (cVar3 != '\0') {
      uVar5 = FUN_18000bfe0();
      pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x2e0);
      FUN_18000c1c0(uVar5,2,&UNK_18014a130,&UNK_180149ce0);
    }
    if (0xf < uStack_50) {
      __3_YAXPEAX_Z(CONCAT71(uStack_67,uStack_68));
    }
    uStack_50 = 0xf;
    uStack_58 = 0;
    uStack_68 = 0;
    if (0xf < uStack_70) {
      __3_YAXPEAX_Z(CONCAT71(uStack_87,uStack_88));
    }
    uStack_70 = 0xf;
    uStack_78 = 0;
    uStack_88 = 0;
    if (0xf < uStack_30) {
      __3_YAXPEAX_Z(apppuStack_48[0]);
    }
  }
  else {
    cVar3 = FUN_18000c2c0(3);
    if (cVar3 != '\0') {
      uVar5 = FUN_18000bfe0();
      puStack_5d0 = (undefined1 *)CONCAT44(puStack_5d0._4_4_,iVar4);
      pppuStack_5d8 = (undefined8 ***)CONCAT44(pppuStack_5d8._4_4_,0x259);
      FUN_18000c1c0(uVar5,3,&UNK_180149d08,&UNK_180149ce0);
    }
    if (0xf < uStack_50) {
      __3_YAXPEAX_Z(CONCAT71(uStack_67,uStack_68));
    }
    uStack_50 = 0xf;
    uStack_58 = 0;
    uStack_68 = 0;
    if (0xf < uStack_70) {
      __3_YAXPEAX_Z(CONCAT71(uStack_87,uStack_88));
    }
    uStack_70 = 0xf;
    uStack_78 = 0;
    uStack_88 = 0;
    if (0xf < uStack_30) {
      __3_YAXPEAX_Z(apppuStack_48[0]);
    }
  }
  FUN_18011da90(uStack_28 ^ (ulonglong)auStack_5f8);
  return;
}

