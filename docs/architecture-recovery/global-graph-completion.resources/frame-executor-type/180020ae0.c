
/* WARNING: Removing unreachable block (ram,0x000180020cc9) */
/* WARNING: Removing unreachable block (ram,0x000180020ccf) */
/* WARNING: Removing unreachable block (ram,0x000180020cdd) */
/* WARNING: Removing unreachable block (ram,0x000180020d27) */
/* WARNING: Removing unreachable block (ram,0x000180020d2d) */
/* WARNING: Removing unreachable block (ram,0x000180020d3b) */

undefined8 * FUN_180020ae0(undefined8 *param_1,longlong *param_2,undefined4 param_3)

{
  int *piVar1;
  longlong *plVar2;
  longlong lVar3;
  undefined8 *puVar4;
  undefined8 uVar5;
  longlong *plVar6;
  undefined8 uStack_90;
  longlong *plStack_88;
  undefined8 uStack_80;
  longlong *plStack_78;
  longlong lStack_70;
  longlong lStack_68;
  undefined8 uStack_60;
  longlong lStack_58;
  undefined8 uStack_50;
  undefined8 uStack_48;
  
  FUN_18001f980();
  *param_1 = &UNK_18021d420;
  puVar4 = (undefined8 *)__2_YAPEAX_K_Z(0x128);
  if (puVar4 == (undefined8 *)0x0) {
    puVar4 = (undefined8 *)0x0;
  }
  else {
    FUN_18001dc50(puVar4);
    *puVar4 = &UNK_180229e38;
    *(undefined4 *)(puVar4 + 6) = 1;
    *(undefined4 *)(puVar4 + 5) = param_3;
  }
  uStack_90 = 0;
  plStack_88 = (longlong *)0x0;
  FUN_180020f50(&uStack_90,puVar4);
  FUN_18001d870(param_1 + 1,&uStack_90);
  plVar2 = plStack_88;
  if (plStack_88 != (longlong *)0x0) {
    LOCK();
    plVar6 = plStack_88 + 1;
    *(int *)plVar6 = (int)*plVar6 + -1;
    UNLOCK();
    if ((int)*plVar6 == 0) {
      (**(code **)*plStack_88)(plStack_88);
      LOCK();
      piVar1 = (int *)((longlong)plVar2 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_88 + 8))();
      }
    }
  }
  puVar4 = (undefined8 *)__2_YAPEAX_K_Z(0x130);
  if (puVar4 == (undefined8 *)0x0) {
    puVar4 = (undefined8 *)0x0;
  }
  else {
    FUN_18001dc50(puVar4);
    *puVar4 = &UNK_180229e20;
    uVar5 = CreateEventW(0,1,0);
    puVar4[0x25] = uVar5;
    *(undefined4 *)(puVar4 + 6) = 2;
    *(undefined4 *)(puVar4 + 5) = param_3;
  }
  uStack_80 = 0;
  plStack_78 = (longlong *)0x0;
  FUN_180020fc0(&uStack_80,puVar4);
  FUN_18001d870(param_1 + 3,&uStack_80);
  plVar2 = plStack_78;
  if (plStack_78 != (longlong *)0x0) {
    LOCK();
    plVar6 = plStack_78 + 1;
    *(int *)plVar6 = (int)*plVar6 + -1;
    UNLOCK();
    if ((int)*plVar6 == 0) {
      (**(code **)*plStack_78)(plStack_78);
      LOCK();
      piVar1 = (int *)((longlong)plVar2 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_78 + 8))();
      }
    }
  }
  plVar2 = param_1 + 5;
  if (plVar2 != param_2) {
    FUN_180020e00(plVar2,*(undefined8 *)*param_2,(undefined8 *)*param_2);
  }
  uStack_50 = 0;
  uStack_48 = 0;
  uVar5 = param_1[3];
  if (param_1[4] != 0) {
    LOCK();
    piVar1 = (int *)(param_1[4] + 8);
    *piVar1 = *piVar1 + 1;
    UNLOCK();
  }
  FUN_180015c40(&uStack_50,uVar5);
  FUN_18001dfd0(param_1[1],&uStack_50);
  plVar6 = *(longlong **)*plVar2;
  if (plVar6 != (longlong *)*plVar2) {
    do {
      lVar3 = plVar6[2];
      if (*(int *)(lVar3 + 0x20) == 0) {
        lStack_68 = plVar6[3];
        if (lStack_68 != 0) {
          LOCK();
          *(int *)(lStack_68 + 8) = *(int *)(lStack_68 + 8) + 1;
          UNLOCK();
        }
        lStack_70 = lVar3;
        FUN_18001dfd0(param_1[1],&lStack_70);
      }
      if (*(int *)(plVar6[2] + 0x50) == 0) {
        lStack_58 = param_1[4];
        uStack_60 = param_1[3];
        if (lStack_58 != 0) {
          LOCK();
          *(int *)(lStack_58 + 8) = *(int *)(lStack_58 + 8) + 1;
          UNLOCK();
        }
        FUN_18001dfd0(plVar6[2],&uStack_60);
      }
      plVar6 = (longlong *)*plVar6;
    } while (plVar6 != (longlong *)*plVar2);
  }
  return param_1;
}

