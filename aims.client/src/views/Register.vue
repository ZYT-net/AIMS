<template>
  <el-card>
    <h2>用户注册</h2>
    <el-form :model="form" label-width="80px">
      <el-form-item label="用户名">
        <el-input v-model="form.userName" />
      </el-form-item>
      <el-form-item label="密码">
        <el-input type="password" v-model="form.password" />
      </el-form-item>
      <el-form-item label="角色">
        <el-select v-model="form.role" placeholder="请选择角色">
          <el-option label="Farmer" value="Farmer" />
          <el-option label="Admin" value="Admin" />
        </el-select>
      </el-form-item>
      <el-form-item label="真实姓名">
        <el-input v-model="form.realName" />
      </el-form-item>
      <el-form-item label="电话">
        <el-input v-model="form.phone" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="register">注册</el-button>
      </el-form-item>
      <el-alert v-if="success" type="success" show-icon :closable="false" title="注册成功" />
      <el-alert v-if="error" type="error" show-icon :closable="false" title="注册失败" />
    </el-form>
  </el-card>
</template>

<script setup>
  import { ref } from 'vue'
  import api from '@/api'

  const form = ref({
    userName: '',
    password: '',
    role: 'Farmer',
    realName: '',
    phone: ''
  })
  const success = ref(false)
  const error = ref('')

  const register = async () => {
    try {
      await api.post('/users', {
        userName: form.value.userName,
        password: form.value.password,
        role: form.value.role,
        realName: form.value.realName,
        phone: form.value.phone
      })
      success.value = true
      form.value = { userName: '', password: '', role: 'Farmer', realName: '', phone: '' }
      setTimeout(() => (success.value = false), 2000)
    } catch (e) {
      error.value = '注册失败，请稍后重试'
      setTimeout(() => (error.value = ''), 2000)
    }
  }
</script>
