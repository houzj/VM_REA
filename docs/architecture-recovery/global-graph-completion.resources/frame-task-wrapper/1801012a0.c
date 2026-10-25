
longlong FUN_1801012a0(longlong param_1,undefined8 *param_2,undefined8 param_3,undefined8 param_4)

{
  undefined8 uVar1;
  code *pcVar2;
  undefined8 *puVar3;
  longlong lVar4;
  undefined8 uVar5;
  
  uVar5 = 0xfffffffffffffffe;
  *(undefined8 *)(param_1 + 0x18) = 0;
  puVar3 = (undefined8 *)__2_YAPEAX_K_Z(0x38);
  if (puVar3 == (undefined8 *)0x0) {
    __Xbad_alloc_std__YAXXZ();
    pcVar2 = (code *)swi(3);
    lVar4 = (*pcVar2)();
    return lVar4;
  }
  *puVar3 = &UNK_18022fb60;
  puVar3[1] = *param_2;
  *(undefined4 *)(puVar3 + 2) = *(undefined4 *)(param_2 + 1);
  puVar3[3] = 0;
  puVar3[4] = 0;
  lVar4 = param_2[3];
  uVar1 = param_2[2];
  if (lVar4 != 0) {
    LOCK();
    *(int *)(lVar4 + 8) = *(int *)(lVar4 + 8) + 1;
    UNLOCK();
  }
  FUN_180015c40(puVar3 + 3,uVar1,lVar4,param_4,uVar5);
  puVar3[5] = param_2[4];
  *(undefined8 **)(param_1 + 0x18) = puVar3;
  return param_1;
}

