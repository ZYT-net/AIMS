<template>
  <el-card>
    <h2>数据导出</h2>
    <el-form :inline="true" @submit.prevent="exportData">
      <el-form-item label="导出类型">
        <el-select v-model="type" placeholder="请选择">
          <el-option label="用户数据" value="users" />
          <el-option label="农事记录" value="farmrecords" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="exportData">导出Excel</el-button>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script setup>
  import { ref } from 'vue'
  import api from '@/api'

  const type = ref('users')

  const exportData = async () => {
    let url = ''
    if (type.value === 'users') url = '/users/export'
    else if (type.value === 'farmrecords') url = '/farmrecords/export'
    else return

    const res = await api.get(url, { responseType: 'blob' })
    const blob = new Blob([res.data], { type: 'text/csv' })
    const link = document.createElement('a')
    link.href = window.URL.createObjectURL(blob)
    link.download = type.value + '.csv'
    link.click()
    window.URL.revokeObjectURL(link.href)
  }
</script>
