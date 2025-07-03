<template>
  <div>
    <h2>用户管理</h2>
    <ul>
      <li v-for="user in users" :key="user.id">
        {{ user.userName }} - {{ user.role }}
        <el-button type="warning" @click="editUser(user)">编辑</el-button>
        <el-button type="danger" @click="deleteUser(user.id)">删除</el-button>
      </li>
    </ul>
    <el-dialog :visible.sync="dialogVisible" title="用户信息">
      <el-form :model="form" label-width="80px">
        <el-form-item label="用户名">
          <el-input v-model="form.userName" />
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
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="saveUser">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import api from '@/api'

  const users = ref([])
  const dialogVisible = ref(false)
  const form = ref({ id: null, userName: '', role: 'Farmer', realName: '', phone: '' })

  onMounted(async () => {
    const res = await api.get('/users')
    users.value = res.data
  })

  const editUser = (user) => {
    form.value = { ...user }
    dialogVisible.value = true
  }

  const saveUser = async () => {
    if (form.value.id) {
      await api.put(`/users/${form.value.id}`, form.value)
    } else {
      await api.post('/users', form.value)
    }
    dialogVisible.value = false
    const res = await api.get('/users')
    users.value = res.data
  }

  const deleteUser = async (id) => {
    await api.delete(`/users/${id}`)
    const res = await api.get('/users')
    users.value = res.data
  }
</script>
