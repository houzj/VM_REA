
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180021590(longlong *param_1,char *param_2)

{
  char cVar1;
  undefined8 uVar2;
  longlong lVar3;
  undefined8 ****ppppuVar4;
  undefined *puVar5;
  undefined1 auStack_a8 [32];
  undefined4 uStack_88;
  undefined8 ***pppuStack_80;
  undefined1 uStack_78;
  undefined7 uStack_77;
  undefined8 uStack_68;
  ulonglong uStack_60;
  undefined8 uStack_58;
  undefined8 ***apppuStack_50 [3];
  ulonglong uStack_38;
  undefined1 auStack_30 [16];
  undefined8 uStack_20;
  undefined8 uStack_18;
  ulonglong uStack_10;
  
  uStack_58 = 0xfffffffffffffffe;
  uStack_10 = _DAT_180192230 ^ (ulonglong)auStack_a8;
  uStack_18 = 0xf;
  lVar3 = 0;
  uStack_20 = 0;
  auStack_30[0] = 0;
  if (*param_2 != '\0') {
    lVar3 = -1;
    do {
      lVar3 = lVar3 + 1;
    } while (param_2[lVar3] != '\0');
  }
  FUN_1800094c0(auStack_30,param_2,lVar3);
  uVar2 = FUN_1800115c0(&uStack_78,auStack_30,&UNK_1801466d8);
  FUN_1800113a0(apppuStack_50,uVar2,&UNK_180146690);
  if (0xf < uStack_60) {
    __3_YAXPEAX_Z(CONCAT71(uStack_77,uStack_78));
  }
  uStack_60 = 0xf;
  uStack_68 = 0;
  uStack_78 = 0;
  ppppuVar4 = apppuStack_50;
  if (0xf < uStack_38) {
    ppppuVar4 = (undefined8 ****)apppuStack_50[0];
  }
  lVar3 = FUN_18008e2b0(ppppuVar4);
  *param_1 = lVar3;
  if (lVar3 == 0) {
    cVar1 = FUN_18000c2c0(4);
    if (cVar1 == '\0') goto LAB_1800241c3;
    ppppuVar4 = apppuStack_50;
    if (0xf < uStack_38) {
      ppppuVar4 = (undefined8 ****)apppuStack_50[0];
    }
    uVar2 = FUN_18000bfe0();
    uStack_88 = 0xbb;
    puVar5 = &UNK_180146700;
    pppuStack_80 = ppppuVar4;
  }
  else {
    cVar1 = FUN_18000c2c0(2);
    if (cVar1 != '\0') {
      uVar2 = FUN_18000bfe0();
      uStack_88 = 0xbf;
      FUN_18000c1c0(uVar2,2,&UNK_1801466e0,&UNK_1801466b8);
    }
    lVar3 = GetProcAddress(*param_1,&UNK_180146740);
    param_1[1] = lVar3;
    if (lVar3 == 0) {
      cVar1 = FUN_18000c2c0(4);
      if (cVar1 == '\0') goto LAB_1800241c3;
      uVar2 = FUN_18000bfe0();
      pppuStack_80 = (undefined8 ***)&UNK_180146740;
      uStack_88 = 0xc1;
    }
    else {
      lVar3 = GetProcAddress(*param_1,&UNK_180146768);
      param_1[2] = lVar3;
      if (lVar3 == 0) {
        cVar1 = FUN_18000c2c0(4);
        if (cVar1 == '\0') goto LAB_1800241c3;
        uVar2 = FUN_18000bfe0();
        pppuStack_80 = (undefined8 ***)&UNK_180146768;
        uStack_88 = 0xc2;
      }
      else {
        lVar3 = GetProcAddress(*param_1,&UNK_180146750);
        param_1[3] = lVar3;
        if (lVar3 == 0) {
          cVar1 = FUN_18000c2c0(4);
          if (cVar1 == '\0') goto LAB_1800241c3;
          uVar2 = FUN_18000bfe0();
          pppuStack_80 = (undefined8 ***)&UNK_180146750;
          uStack_88 = 0xc3;
        }
        else {
          lVar3 = GetProcAddress(*param_1,&UNK_180146798);
          param_1[4] = lVar3;
          if (lVar3 == 0) {
            cVar1 = FUN_18000c2c0(4);
            if (cVar1 == '\0') goto LAB_1800241c3;
            uVar2 = FUN_18000bfe0();
            pppuStack_80 = (undefined8 ***)&UNK_180146798;
            uStack_88 = 0xc4;
          }
          else {
            lVar3 = GetProcAddress(*param_1,&UNK_180146780);
            param_1[5] = lVar3;
            if (lVar3 == 0) {
              cVar1 = FUN_18000c2c0(4);
              if (cVar1 == '\0') goto LAB_1800241c3;
              uVar2 = FUN_18000bfe0();
              pppuStack_80 = (undefined8 ***)&UNK_180146780;
              uStack_88 = 0xc5;
            }
            else {
              lVar3 = GetProcAddress(*param_1,&UNK_1801467d0);
              param_1[6] = lVar3;
              if (lVar3 == 0) {
                cVar1 = FUN_18000c2c0(4);
                if (cVar1 == '\0') goto LAB_1800241c3;
                uVar2 = FUN_18000bfe0();
                pppuStack_80 = (undefined8 ***)&UNK_1801467d0;
                uStack_88 = 0xc6;
              }
              else {
                lVar3 = GetProcAddress(*param_1,&UNK_1801467b0);
                param_1[7] = lVar3;
                if (lVar3 == 0) {
                  cVar1 = FUN_18000c2c0(4);
                  if (cVar1 == '\0') goto LAB_1800241c3;
                  uVar2 = FUN_18000bfe0();
                  pppuStack_80 = (undefined8 ***)&UNK_1801467b0;
                  uStack_88 = 199;
                }
                else {
                  lVar3 = GetProcAddress(*param_1,&UNK_180146808);
                  param_1[8] = lVar3;
                  if (lVar3 == 0) {
                    cVar1 = FUN_18000c2c0(4);
                    if (cVar1 == '\0') goto LAB_1800241c3;
                    uVar2 = FUN_18000bfe0();
                    pppuStack_80 = (undefined8 ***)&UNK_180146808;
                    uStack_88 = 200;
                  }
                  else {
                    lVar3 = GetProcAddress(*param_1,&UNK_1801467e8);
                    param_1[9] = lVar3;
                    if (lVar3 == 0) {
                      cVar1 = FUN_18000c2c0(4);
                      if (cVar1 == '\0') goto LAB_1800241c3;
                      uVar2 = FUN_18000bfe0();
                      pppuStack_80 = (undefined8 ***)&UNK_1801467e8;
                      uStack_88 = 0xc9;
                    }
                    else {
                      lVar3 = GetProcAddress(*param_1,&UNK_180146840);
                      param_1[10] = lVar3;
                      if (lVar3 == 0) {
                        cVar1 = FUN_18000c2c0(4);
                        if (cVar1 == '\0') goto LAB_1800241c3;
                        uVar2 = FUN_18000bfe0();
                        pppuStack_80 = (undefined8 ***)&UNK_180146840;
                        uStack_88 = 0xca;
                      }
                      else {
                        lVar3 = GetProcAddress(*param_1,&UNK_180146828);
                        param_1[0xb] = lVar3;
                        if (lVar3 == 0) {
                          cVar1 = FUN_18000c2c0(4);
                          if (cVar1 == '\0') goto LAB_1800241c3;
                          uVar2 = FUN_18000bfe0();
                          pppuStack_80 = (undefined8 ***)&UNK_180146828;
                          uStack_88 = 0xcb;
                        }
                        else {
                          lVar3 = GetProcAddress(*param_1,&UNK_180146878);
                          param_1[0xc] = lVar3;
                          if (lVar3 == 0) {
                            cVar1 = FUN_18000c2c0(4);
                            if (cVar1 == '\0') goto LAB_1800241c3;
                            uVar2 = FUN_18000bfe0();
                            pppuStack_80 = (undefined8 ***)&UNK_180146878;
                            uStack_88 = 0xcc;
                          }
                          else {
                            lVar3 = GetProcAddress(*param_1,&UNK_180146860);
                            param_1[0xd] = lVar3;
                            if (lVar3 == 0) {
                              cVar1 = FUN_18000c2c0(4);
                              if (cVar1 == '\0') goto LAB_1800241c3;
                              uVar2 = FUN_18000bfe0();
                              pppuStack_80 = (undefined8 ***)&UNK_180146860;
                              uStack_88 = 0xcd;
                            }
                            else {
                              lVar3 = GetProcAddress(*param_1,&UNK_1801468b0);
                              param_1[0xe] = lVar3;
                              if (lVar3 == 0) {
                                cVar1 = FUN_18000c2c0(4);
                                if (cVar1 == '\0') goto LAB_1800241c3;
                                uVar2 = FUN_18000bfe0();
                                pppuStack_80 = (undefined8 ***)&UNK_1801468b0;
                                uStack_88 = 0xce;
                              }
                              else {
                                lVar3 = GetProcAddress(*param_1,&UNK_180146890);
                                param_1[0xf] = lVar3;
                                if (lVar3 == 0) {
                                  cVar1 = FUN_18000c2c0(4);
                                  if (cVar1 == '\0') goto LAB_1800241c3;
                                  uVar2 = FUN_18000bfe0();
                                  pppuStack_80 = (undefined8 ***)&UNK_180146890;
                                  uStack_88 = 0xcf;
                                }
                                else {
                                  lVar3 = GetProcAddress(*param_1,&UNK_1801468e0);
                                  param_1[0x10] = lVar3;
                                  if (lVar3 == 0) {
                                    cVar1 = FUN_18000c2c0(4);
                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                    uVar2 = FUN_18000bfe0();
                                    pppuStack_80 = (undefined8 ***)&UNK_1801468e0;
                                    uStack_88 = 0xd0;
                                  }
                                  else {
                                    lVar3 = GetProcAddress(*param_1,&UNK_1801468c8);
                                    param_1[0x11] = lVar3;
                                    if (lVar3 == 0) {
                                      cVar1 = FUN_18000c2c0(4);
                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                      uVar2 = FUN_18000bfe0();
                                      pppuStack_80 = (undefined8 ***)&UNK_1801468c8;
                                      uStack_88 = 0xd1;
                                    }
                                    else {
                                      lVar3 = GetProcAddress(*param_1,&UNK_180146910);
                                      param_1[0x12] = lVar3;
                                      if (lVar3 == 0) {
                                        cVar1 = FUN_18000c2c0(4);
                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                        uVar2 = FUN_18000bfe0();
                                        pppuStack_80 = (undefined8 ***)&UNK_180146910;
                                        uStack_88 = 0xd2;
                                      }
                                      else {
                                        lVar3 = GetProcAddress(*param_1,&UNK_1801468f8);
                                        param_1[0x13] = lVar3;
                                        if (lVar3 == 0) {
                                          cVar1 = FUN_18000c2c0(4);
                                          if (cVar1 == '\0') goto LAB_1800241c3;
                                          uVar2 = FUN_18000bfe0();
                                          pppuStack_80 = (undefined8 ***)&UNK_1801468f8;
                                          uStack_88 = 0xd3;
                                        }
                                        else {
                                          lVar3 = GetProcAddress(*param_1,&UNK_180146948);
                                          param_1[0x14] = lVar3;
                                          if (lVar3 == 0) {
                                            cVar1 = FUN_18000c2c0(4);
                                            if (cVar1 == '\0') goto LAB_1800241c3;
                                            uVar2 = FUN_18000bfe0();
                                            pppuStack_80 = (undefined8 ***)&UNK_180146948;
                                            uStack_88 = 0xd4;
                                          }
                                          else {
                                            lVar3 = GetProcAddress(*param_1,&UNK_180146928);
                                            param_1[0x15] = lVar3;
                                            if (lVar3 == 0) {
                                              cVar1 = FUN_18000c2c0(4);
                                              if (cVar1 == '\0') goto LAB_1800241c3;
                                              uVar2 = FUN_18000bfe0();
                                              pppuStack_80 = (undefined8 ***)&UNK_180146928;
                                              uStack_88 = 0xd5;
                                            }
                                            else {
                                              lVar3 = GetProcAddress(*param_1,&UNK_180146978);
                                              param_1[0x16] = lVar3;
                                              if (lVar3 == 0) {
                                                cVar1 = FUN_18000c2c0(4);
                                                if (cVar1 == '\0') goto LAB_1800241c3;
                                                uVar2 = FUN_18000bfe0();
                                                pppuStack_80 = (undefined8 ***)&UNK_180146978;
                                                uStack_88 = 0xd6;
                                              }
                                              else {
                                                lVar3 = GetProcAddress(*param_1,&UNK_180146960);
                                                param_1[0x17] = lVar3;
                                                if (lVar3 == 0) {
                                                  cVar1 = FUN_18000c2c0(4);
                                                  if (cVar1 == '\0') goto LAB_1800241c3;
                                                  uVar2 = FUN_18000bfe0();
                                                  pppuStack_80 = (undefined8 ***)&UNK_180146960;
                                                  uStack_88 = 0xd7;
                                                }
                                                else {
                                                  lVar3 = GetProcAddress(*param_1,&UNK_1801469b0);
                                                  param_1[0x18] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801469b0;
                                                    uStack_88 = 0xd8;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146998);
                                                    param_1[0x19] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146998;
                                                      uStack_88 = 0xd9;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_1801469e8
                                                                            );
                                                      param_1[0x1a] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_1801469e8;
                                                        uStack_88 = 0xda;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_1801469d0);
                                                  param_1[0x1b] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801469d0;
                                                    uStack_88 = 0xdb;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146a18);
                                                    param_1[0x1c] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146a18;
                                                      uStack_88 = 0xdc;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146a00
                                                                            );
                                                      param_1[0x1d] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146a00;
                                                        uStack_88 = 0xdd;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146a48);
                                                  param_1[0x1e] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146a48;
                                                    uStack_88 = 0xde;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146a30);
                                                    param_1[0x1f] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146a30;
                                                      uStack_88 = 0xdf;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146a78
                                                                            );
                                                      param_1[0x20] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146a78;
                                                        uStack_88 = 0xe0;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146a60);
                                                  param_1[0x21] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146a60;
                                                    uStack_88 = 0xe1;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146aa8);
                                                    param_1[0x22] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146aa8;
                                                      uStack_88 = 0xe2;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146a88
                                                                            );
                                                      param_1[0x23] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146a88;
                                                        uStack_88 = 0xe3;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146ad8);
                                                  param_1[0x24] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146ad8;
                                                    uStack_88 = 0xe4;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146ac0);
                                                    param_1[0x25] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146ac0;
                                                      uStack_88 = 0xe5;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146b10
                                                                            );
                                                      param_1[0x26] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146b10;
                                                        uStack_88 = 0xe6;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146af0);
                                                  param_1[0x27] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146af0;
                                                    uStack_88 = 0xe7;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146b48);
                                                    param_1[0x28] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146b48;
                                                      uStack_88 = 0xe8;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146b30
                                                                            );
                                                      param_1[0x29] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146b30;
                                                        uStack_88 = 0xe9;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146b78);
                                                  param_1[0x2a] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146b78;
                                                    uStack_88 = 0xea;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146b60);
                                                    param_1[0x2b] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146b60;
                                                      uStack_88 = 0xeb;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146bb0
                                                                            );
                                                      param_1[0x2c] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146bb0;
                                                        uStack_88 = 0xec;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146b98);
                                                  param_1[0x2d] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146b98;
                                                    uStack_88 = 0xed;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146be8);
                                                    param_1[0x2e] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146be8;
                                                      uStack_88 = 0xee;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146bc8
                                                                            );
                                                      param_1[0x2f] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146bc8;
                                                        uStack_88 = 0xef;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146c10);
                                                  param_1[0x30] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146c10;
                                                    uStack_88 = 0xf0;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146c00);
                                                    param_1[0x31] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146c00;
                                                      uStack_88 = 0xf1;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146c38
                                                                            );
                                                      param_1[0x32] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146c38;
                                                        uStack_88 = 0xf2;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146c20);
                                                  param_1[0x33] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146c20;
                                                    uStack_88 = 0xf3;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146c58);
                                                    param_1[0x34] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146c58;
                                                      uStack_88 = 0xf4;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146c48
                                                                            );
                                                      param_1[0x35] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146c48;
                                                        uStack_88 = 0xf5;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146c80);
                                                  param_1[0x36] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146c80;
                                                    uStack_88 = 0xf6;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146c68);
                                                    param_1[0x37] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146c68;
                                                      uStack_88 = 0xf7;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146ca8
                                                                            );
                                                      param_1[0x38] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146ca8;
                                                        uStack_88 = 0xf8;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146c90);
                                                  param_1[0x39] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146c90;
                                                    uStack_88 = 0xf9;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146cd8);
                                                    param_1[0x3a] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146cd8;
                                                      uStack_88 = 0xfa;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146cc0
                                                                            );
                                                      param_1[0x3b] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146cc0;
                                                        uStack_88 = 0xfb;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146d10);
                                                  param_1[0x3c] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146d10;
                                                    uStack_88 = 0xfc;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146cf0);
                                                    param_1[0x3d] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146cf0;
                                                      uStack_88 = 0xfd;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146d40
                                                                            );
                                                      param_1[0x3e] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146d40;
                                                        uStack_88 = 0xfe;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146d28);
                                                  param_1[0x3f] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146d28;
                                                    uStack_88 = 0xff;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146d78);
                                                    param_1[0x40] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146d78;
                                                      uStack_88 = 0x100;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146d60
                                                                            );
                                                      param_1[0x41] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146d60;
                                                        uStack_88 = 0x101;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146da0);
                                                  param_1[0x42] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146da0;
                                                    uStack_88 = 0x102;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146d90);
                                                    param_1[0x43] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146d90;
                                                      uStack_88 = 0x103;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146dd8
                                                                            );
                                                      param_1[0x44] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146dd8;
                                                        uStack_88 = 0x104;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146dc0);
                                                  param_1[0x45] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146dc0;
                                                    uStack_88 = 0x105;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146e08);
                                                    param_1[0x46] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146e08;
                                                      uStack_88 = 0x106;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146df0
                                                                            );
                                                      param_1[0x47] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146df0;
                                                        uStack_88 = 0x107;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146e38);
                                                  param_1[0x48] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146e38;
                                                    uStack_88 = 0x108;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146e20);
                                                    param_1[0x49] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146e20;
                                                      uStack_88 = 0x109;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146e70
                                                                            );
                                                      param_1[0x4a] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146e70;
                                                        uStack_88 = 0x10a;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146e50);
                                                  param_1[0x4b] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146e50;
                                                    uStack_88 = 0x10b;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146ea8);
                                                    param_1[0x4c] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146ea8;
                                                      uStack_88 = 0x10c;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146e90
                                                                            );
                                                      param_1[0x4d] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146e90;
                                                        uStack_88 = 0x10d;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146ee8);
                                                  param_1[0x4e] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146ee8;
                                                    uStack_88 = 0x10e;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146ec8);
                                                    param_1[0x4f] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146ec8;
                                                      uStack_88 = 0x10f;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146f20
                                                                            );
                                                      param_1[0x50] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146f20;
                                                        uStack_88 = 0x110;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146f08);
                                                  param_1[0x51] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146f08;
                                                    uStack_88 = 0x111;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146f58);
                                                    param_1[0x52] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146f58;
                                                      uStack_88 = 0x112;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146f40
                                                                            );
                                                      param_1[0x53] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146f40;
                                                        uStack_88 = 0x113;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146f98);
                                                  param_1[0x54] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146f98;
                                                    uStack_88 = 0x114;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180146f70);
                                                    param_1[0x55] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180146f70;
                                                      uStack_88 = 0x115;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146fd0
                                                                            );
                                                      param_1[0x56] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146fd0;
                                                        uStack_88 = 0x116;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180146fb8);
                                                  param_1[0x57] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180146fb8;
                                                    uStack_88 = 0x117;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180147000);
                                                    param_1[0x58] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180147000;
                                                      uStack_88 = 0x118;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180146fe8
                                                                            );
                                                      param_1[0x59] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180146fe8;
                                                        uStack_88 = 0x119;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180147030);
                                                  param_1[0x5a] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147030;
                                                    uStack_88 = 0x11a;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180147018);
                                                    param_1[0x5b] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180147018;
                                                      uStack_88 = 0x11b;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180147060
                                                                            );
                                                      param_1[0x5c] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180147060;
                                                        uStack_88 = 0x11c;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_180147048);
                                                  param_1[0x5d] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147048;
                                                    uStack_88 = 0x11d;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_180147090);
                                                    param_1[0x5e] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_180147090;
                                                      uStack_88 = 0x11e;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180147078
                                                                            );
                                                      param_1[0x5f] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180147078;
                                                        uStack_88 = 0x11f;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_1801470c8);
                                                  param_1[0x60] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801470c8;
                                                    uStack_88 = 0x120;
                                                  }
                                                  else {
                                                    lVar3 = GetProcAddress(*param_1,&UNK_1801470a8);
                                                    param_1[0x61] = lVar3;
                                                    if (lVar3 == 0) {
                                                      cVar1 = FUN_18000c2c0(4);
                                                      if (cVar1 == '\0') goto LAB_1800241c3;
                                                      uVar2 = FUN_18000bfe0();
                                                      pppuStack_80 = (undefined8 ***)&UNK_1801470a8;
                                                      uStack_88 = 0x121;
                                                    }
                                                    else {
                                                      lVar3 = GetProcAddress(*param_1,&UNK_180147100
                                                                            );
                                                      param_1[0x62] = lVar3;
                                                      if (lVar3 == 0) {
                                                        cVar1 = FUN_18000c2c0(4);
                                                        if (cVar1 == '\0') goto LAB_1800241c3;
                                                        uVar2 = FUN_18000bfe0();
                                                        pppuStack_80 = (undefined8 ***)
                                                                       &UNK_180147100;
                                                        uStack_88 = 0x122;
                                                      }
                                                      else {
                                                        lVar3 = GetProcAddress(*param_1,&
                                                  UNK_1801470e0);
                                                  param_1[99] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801470e0;
                                                    uStack_88 = 0x123;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147140);
                                                  param_1[100] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147140;
                                                    uStack_88 = 0x124;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147120);
                                                  param_1[0x65] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147120;
                                                    uStack_88 = 0x125;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147180);
                                                  param_1[0x66] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147180;
                                                    uStack_88 = 0x126;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147160);
                                                  param_1[0x67] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147160;
                                                    uStack_88 = 0x127;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801471c0);
                                                  param_1[0x68] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801471c0;
                                                    uStack_88 = 0x128;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801471a0);
                                                  param_1[0x69] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801471a0;
                                                    uStack_88 = 0x129;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801471f8);
                                                  param_1[0x6a] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801471f8;
                                                    uStack_88 = 0x12a;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801471d8);
                                                  param_1[0x6b] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801471d8;
                                                    uStack_88 = 299;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147230);
                                                  param_1[0x6c] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147230;
                                                    uStack_88 = 300;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147210);
                                                  param_1[0x6d] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147210;
                                                    uStack_88 = 0x12d;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147278);
                                                  param_1[0x6e] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147278;
                                                    uStack_88 = 0x12e;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147250);
                                                  param_1[0x6f] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147250;
                                                    uStack_88 = 0x12f;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801472c0);
                                                  param_1[0x70] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801472c0;
                                                    uStack_88 = 0x130;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147298);
                                                  param_1[0x71] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147298;
                                                    uStack_88 = 0x131;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147308);
                                                  param_1[0x72] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147308;
                                                    uStack_88 = 0x132;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801472e8);
                                                  param_1[0x73] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801472e8;
                                                    uStack_88 = 0x133;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147348);
                                                  param_1[0x74] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147348;
                                                    uStack_88 = 0x134;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147330);
                                                  param_1[0x75] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147330;
                                                    uStack_88 = 0x135;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147378);
                                                  param_1[0x76] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147378;
                                                    uStack_88 = 0x136;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147360);
                                                  param_1[0x77] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147360;
                                                    uStack_88 = 0x137;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801473b8);
                                                  param_1[0x78] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801473b8;
                                                    uStack_88 = 0x138;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147390);
                                                  param_1[0x79] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147390;
                                                    uStack_88 = 0x139;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801473f8);
                                                  param_1[0x7a] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801473f8;
                                                    uStack_88 = 0x13a;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801473e0);
                                                  param_1[0x7b] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801473e0;
                                                    uStack_88 = 0x13b;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147430);
                                                  param_1[0x7c] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147430;
                                                    uStack_88 = 0x13c;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147410);
                                                  param_1[0x7d] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147410;
                                                    uStack_88 = 0x13d;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147470);
                                                  param_1[0x7e] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147470;
                                                    uStack_88 = 0x13e;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147450);
                                                  param_1[0x7f] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147450;
                                                    uStack_88 = 0x13f;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801474b0);
                                                  param_1[0x80] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801474b0;
                                                    uStack_88 = 0x140;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147490);
                                                  param_1[0x81] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147490;
                                                    uStack_88 = 0x141;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801474f0);
                                                  param_1[0x82] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801474f0;
                                                    uStack_88 = 0x142;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801474d0);
                                                  param_1[0x83] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801474d0;
                                                    uStack_88 = 0x143;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147530);
                                                  param_1[0x84] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147530;
                                                    uStack_88 = 0x144;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147510);
                                                  param_1[0x85] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147510;
                                                    uStack_88 = 0x145;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147578);
                                                  param_1[0x86] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147578;
                                                    uStack_88 = 0x146;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147550);
                                                  param_1[0x87] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147550;
                                                    uStack_88 = 0x147;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801475b8);
                                                  param_1[0x88] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801475b8;
                                                    uStack_88 = 0x148;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801475a0);
                                                  param_1[0x89] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801475a0;
                                                    uStack_88 = 0x149;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147600);
                                                  param_1[0x8a] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147600;
                                                    uStack_88 = 0x14a;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801475e8);
                                                  param_1[0x8b] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801475e8;
                                                    uStack_88 = 0x14b;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147640);
                                                  param_1[0x8c] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147640;
                                                    uStack_88 = 0x14c;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147620);
                                                  param_1[0x8d] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147620;
                                                    uStack_88 = 0x14d;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147688);
                                                  param_1[0x8e] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147688;
                                                    uStack_88 = 0x14e;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147668);
                                                  param_1[0x8f] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147668;
                                                    uStack_88 = 0x14f;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801476c0);
                                                  param_1[0x90] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801476c0;
                                                    uStack_88 = 0x150;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801476a0);
                                                  param_1[0x91] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801476a0;
                                                    uStack_88 = 0x151;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801476f8);
                                                  param_1[0x92] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801476f8;
                                                    uStack_88 = 0x152;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801476e0);
                                                  param_1[0x93] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801476e0;
                                                    uStack_88 = 0x153;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147738);
                                                  param_1[0x94] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147738;
                                                    uStack_88 = 0x154;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147710);
                                                  param_1[0x95] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147710;
                                                    uStack_88 = 0x155;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147770);
                                                  param_1[0x96] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147770;
                                                    uStack_88 = 0x156;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147758);
                                                  param_1[0x97] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147758;
                                                    uStack_88 = 0x157;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801477b0);
                                                  param_1[0x98] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801477b0;
                                                    uStack_88 = 0x158;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147790);
                                                  param_1[0x99] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147790;
                                                    uStack_88 = 0x159;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801477f0);
                                                  param_1[0x9a] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801477f0;
                                                    uStack_88 = 0x15a;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_1801477d0);
                                                  param_1[0x9c] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_1801477d0;
                                                    uStack_88 = 0x15b;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147840);
                                                  param_1[0x9b] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147840;
                                                    uStack_88 = 0x15c;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147810);
                                                  param_1[0x9d] = lVar3;
                                                  if (lVar3 == 0) {
                                                    cVar1 = FUN_18000c2c0(4);
                                                    if (cVar1 == '\0') goto LAB_1800241c3;
                                                    uVar2 = FUN_18000bfe0();
                                                    pppuStack_80 = (undefined8 ***)&UNK_180147810;
                                                    uStack_88 = 0x15e;
                                                  }
                                                  else {
                                                    lVar3 = func_0x00018008e340(*param_1,&
                                                  UNK_180147860);
                                                  param_1[0x9e] = lVar3;
                                                  if ((lVar3 != 0) ||
                                                     (cVar1 = FUN_18000c2c0(4), cVar1 == '\0'))
                                                  goto LAB_1800241c3;
                                                  uVar2 = FUN_18000bfe0();
                                                  pppuStack_80 = (undefined8 ***)&UNK_180147860;
                                                  uStack_88 = 0x15f;
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                  }
                                                }
                                              }
                                            }
                                          }
                                        }
                                      }
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
    puVar5 = &UNK_180146720;
  }
  FUN_18000c1c0(uVar2,4,puVar5,&UNK_1801466b8);
LAB_1800241c3:
  FUN_180009450(apppuStack_50);
  FUN_180009450(auStack_30);
  FUN_18011da90(uStack_10 ^ (ulonglong)auStack_a8);
  return;
}

