<!-- eslint-disable vue/multi-word-component-names -->
<template>
  <el-card>
    <div v-if="user">
      <p>用户名：{{ user.userName }}</p>
      <p>真实姓名：{{ user.realName }}</p>
      <p>电话：{{ user.phone }}</p>
      <p>角色：{{ user.role }}</p>
    </div>
  </el-card>
</template>
<script setup>
  import { ref, onMounted } from 'vue'
  import api from '@/api'

  const user = ref(null)
  onMounted(() => {
    const userStr = localStorage.getItem('user')
    if (userStr) {
      user.value = JSON.parse(userStr)
    }
  })
</script>
