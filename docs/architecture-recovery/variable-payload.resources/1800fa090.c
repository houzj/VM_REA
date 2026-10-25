
void FUN_1800fa090(undefined8 param_1,undefined8 param_2,undefined8 param_3,undefined8 param_4)

{
  undefined8 *puVar1;
  undefined8 uVar2;
  
  uVar2 = 0xfffffffffffffffe;
  puVar1 = (undefined8 *)__2_YAPEAX_K_Z(0x18);
  if (puVar1 != (undefined8 *)0x0) {
    *(undefined4 *)(puVar1 + 1) = 1;
    *(undefined4 *)((longlong)puVar1 + 0xc) = 1;
    *puVar1 = &DAT_18022e538;
    puVar1[2] = param_2;
  }
  FUN_180015c40(param_1,param_2,puVar1,param_4,uVar2);
  return;
}

