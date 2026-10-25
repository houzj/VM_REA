
/* WARNING: Function: __security_check_cookie replaced with injection: security_check_cookie */

int FUN_1800249ea(void)

{
  byte bVar1;
  int iVar2;
  uint uVar3;
  longlong lVar4;
  byte *pbVar5;
  longlong in_stack_00000058;
  int in_stack_00000060;
  int in_stack_00000168;
  longlong in_stack_000019b0;
  
  lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
  basic_string<>((_String_alloc<0,std::_String_base_types<char,std::allocator<char>_>_> *)
                 &stack0x000011c0,(char *)(lVar4 + 0x129));
  in_stack_00000168 = *(int *)(in_stack_00000058 + 0x304);
  lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
  if (*(char *)(lVar4 + 0x448) == '\0') {
    basic_string<>((_String_alloc<0,std::_String_base_types<char,std::allocator<char>_>_> *)
                   &stack0x000011a0,(char *)(in_stack_00000058 + 0x204));
    lVar4 = FUN_180003ee0((longlong)&stack0x000011a0);
    if (lVar4 != 0) {
      lVar4 = FUN_180003ec0((_String_val<std::_Simple_types<char>_> *)&stack0x000011a0);
      pbVar5 = (byte *)FUN_180003ec0((_String_val<std::_Simple_types<char>_> *)&stack0x000011c0);
      lVar4 = lVar4 - (longlong)pbVar5;
      do {
        bVar1 = *pbVar5;
        if (bVar1 != pbVar5[lVar4]) {
          uVar3 = -(uint)(bVar1 < pbVar5[lVar4]) | 1;
          goto LAB_180024ad3;
        }
        pbVar5 = pbVar5 + 1;
      } while (bVar1 != 0);
      uVar3 = 0;
LAB_180024ad3:
      if (uVar3 != 0) {
        ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                        &stack0x000011a0);
        ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                        &stack0x000011c0);
        FUN_180016480((longlong)&stack0x00000048);
        ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                        &stack0x00001080);
        ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                        &stack0x00001060);
        ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                        &stack0x00001020);
        FUN_18001b230((longlong *)&stack0x00000070);
        return -0x1ffffcf1;
      }
    }
    pbVar5 = (byte *)FUN_180003ec0((_String_val<std::_Simple_types<char>_> *)&stack0x000011c0);
    lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
    in_stack_00000060 =
         FUN_180028c30(in_stack_000019b0,(char *)(lVar4 + 0x29),pbVar5,
                       *(float **)(in_stack_00000058 + 0x308),in_stack_00000168);
    ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                    &stack0x000011a0);
  }
  else {
    in_stack_00000060 =
         FUN_180028250(in_stack_000019b0,*(longlong *)(in_stack_00000058 + 0x308),in_stack_00000168)
    ;
  }
  ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                  &stack0x000011c0);
  iVar2 = in_stack_00000060;
  FUN_180016480((longlong)&stack0x00000048);
  ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                  &stack0x00001080);
  ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                  &stack0x00001060);
  ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                  &stack0x00001020);
  FUN_18001b230((longlong *)&stack0x00000070);
  return iVar2;
}

