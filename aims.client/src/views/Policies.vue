<template>
  <div>
    <h2>政策浏览</h2>
    <ul>
      <li v-for="policy in policies" :key="policy.id">
        <b>{{ policy.title }}</b> - {{ policy.publishDate }}<br />
        {{ policy.content }}
      </li>
    </ul>
    <PolicyPush />
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import api from '@/api'
  import PolicyPush from '@/components/PolicyPush.vue'

  const policies = ref([])

  onMounted(async () => {
    const res = await api.get('/policies')
    policies.value = res.data
  })
</script>
