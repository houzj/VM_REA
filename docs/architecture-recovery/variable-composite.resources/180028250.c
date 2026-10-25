
int FUN_180028250(longlong param_1,longlong param_2,int param_3)

{
  int iVar1;
  undefined1 uVar2;
  undefined4 in_register_00000084;
  int local_34;
  ulonglong local_28;
  ulonglong local_20;
  
  local_34 = 0;
  uVar2 = VM_M_SetGlobalVarLockStatus
                    (*(undefined4 *)(param_1 + 0x48),*(undefined4 *)(param_1 + 0x48),
                     CONCAT71((int7)(CONCAT44(in_register_00000084,param_3) >> 8),1));
  for (local_28 = 0; local_28 < (ulonglong)(longlong)param_3; local_28 = local_28 + 1) {
    iVar1 = *(int *)(param_2 + 0x100 + local_28 * 0x130);
    if (iVar1 == 0) {
      if (*(longlong *)(param_2 + 0x108 + local_28 * 0x130) == 0) {
        local_34 = VM_M_SetGlobalVarInt(*(undefined8 *)(param_1 + 8),param_2 + local_28 * 0x130,0,0)
        ;
      }
      else {
        local_34 = FUN_1800295b0(param_1,(char *)(param_2 + local_28 * 0x130),
                                 *(longlong *)(param_2 + 0x108 + local_28 * 0x130),
                                 *(int *)(param_2 + 0x104 + local_28 * 0x130),uVar2,'\x01');
      }
    }
    else if (iVar1 == 1) {
      if (*(longlong *)(param_2 + 0x110 + local_28 * 0x130) == 0) {
        local_34 = VM_M_SetGlobalVarFloat
                             (*(undefined8 *)(param_1 + 8),param_2 + local_28 * 0x130,0,0);
      }
      else {
        local_34 = FUN_180029eb0(param_1,(char *)(param_2 + local_28 * 0x130),
                                 *(float **)(param_2 + 0x110 + local_28 * 0x130),
                                 *(int *)(param_2 + 0x104 + local_28 * 0x130),uVar2,'\x01');
      }
    }
    else if (iVar1 == 2) {
      if (*(longlong *)(param_2 + 0x118 + local_28 * 0x130) == 0) {
        local_34 = VM_M_SetGlobalVarString(*(undefined8 *)(param_1 + 8),param_2 + local_28 * 0x130);
      }
      else {
        local_34 = FUN_18002a7b0(param_1,(char *)(param_2 + local_28 * 0x130),
                                 *(longlong *)(param_2 + 0x118 + local_28 * 0x130),
                                 *(int *)(param_2 + 0x104 + local_28 * 0x130),uVar2,'\x01');
      }
    }
    else if ((iVar1 == 4) && (*(longlong *)(param_2 + 0x118 + local_28 * 0x130) != 0)) {
      for (local_20 = 0;
          local_20 < (ulonglong)(longlong)*(int *)(param_2 + 0x104 + local_28 * 0x130);
          local_20 = local_20 + 1) {
        local_34 = FUN_18002b2e0(param_1,(char *)(param_2 + local_28 * 0x130),
                                 *(void **)(*(longlong *)(param_2 + 0x118 + local_28 * 0x130) +
                                           local_20 * 0x60),
                                 *(int *)(*(longlong *)(param_2 + 0x118 + local_28 * 0x130) + 8 +
                                         local_20 * 0x60),(undefined4)local_20);
      }
    }
  }
  VM_M_SetGlobalVarLockStatus(*(undefined4 *)(param_1 + 0x48),*(undefined4 *)(param_1 + 0x48),0);
  return local_34;
}

